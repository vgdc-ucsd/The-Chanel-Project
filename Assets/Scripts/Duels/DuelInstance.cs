using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum Team
{
    Player, Enemy, Neutral
}

public class DuelInstance
{
    private bool mainDuel;
    private UIManager ui;

    private Board board;
    private DuelSettings settings;
    private PlayerSettings playerSettings, enemySettings;
    public CharStatus PlayerStatus, EnemyStatus;
    //private int turnNumber = 1;

    public DuelInstance(CharStatus player, CharStatus enemy, bool mainDuel) {
        // only use UI if main duel is true
        this.mainDuel = mainDuel;
        if(mainDuel) ui = DuelManager.Instance.UI;
        else ui = null;

        // set settings and status
        settings = DuelManager.Instance.Settings;
        playerSettings = settings.Player;
        enemySettings = settings.Enemy;
        PlayerStatus = player;
        EnemyStatus = enemy;

        if(mainDuel) {
            // initialize
            PlayerStatus.Init(Team.Player);
            EnemyStatus.Init(Team.Enemy);
            PlayerStatus.SetDeck(DuelManager.Instance.PlayerDeck);
            EnemyStatus.SetDeck(DuelManager.Instance.EnemyDeck);

            // Draw staring cards
            DrawCards(Team.Player, playerSettings.StartingCards); // TODO remove
            DrawCards(Team.Enemy, enemySettings.StartingCards);
        }
    }

    public DuelInstance Clone() {
        return new DuelInstance(PlayerStatus.Clone(), EnemyStatus.Clone(), false);
    }

    public void InitBoard(Board board)
    {
        this.board = board;
    }

    public void ProcessBoard(Board board, Team team) {
        // the team is whoever just activated end turn

        this.board = board;

        // Process all cards
        for(int i = 0; i < board.Cols; i++) {
            for(int j = 0; j < board.Rows; j++) {
                BoardCoords pos = new BoardCoords(i,j);
                if (board.IsOccupied(pos)) {
                    ProcessCard(board.GetCard(pos), pos, team);
                }
            }
        }

        EndTurn(team);
        
        // UI
        if(mainDuel) {
            DuelEvents.Instance.UpdateUI();
            if(team == Team.Player) DuelEvents.Instance.UpdateHand();
        }
    }

    private void ProcessCard(UnitCard card, BoardCoords pos, Team team) {
        if (card == null) {
            Debug.LogError("Tried to process null card!");
            return;
        }

        // Cards only take actions on their turn
        if (card.team == team) {
            // Activate abilities        
            foreach(Ability a in card.Abilities) {
                // Only activate if the activation condition is OnProcess
                if(a != null && a.Condition == ActivationCondition.OnProcess) {
                    a.Activate(board, card);
                }
            }

            // Attack
            foreach(Attack atk in card.Attacks) {
                ProcessAttack(card, atk);
            }
        }
    }

    private void ProcessAttack(UnitCard card, Attack atk) {
        if (atk == null) {
            Debug.Log("Tried to process null attack");
            return;
        }

        BoardCoords atkDest = card.pos + new BoardCoords(atk.direction);

        // Attack targeting enemy
        if(board.BeyondEnemyEdge(atkDest) && card.team == Team.Player) {
            EnemyStatus.DealDamage(atk.damage);
            return;
        }

        // Attack targeting player
        if(board.BeyondPlayerEdge(atkDest) && card.team == Team.Enemy) {
            PlayerStatus.DealDamage(atk.damage);
            return;
        }
        
        // Do nothing if attack is out of bounds
        if(board.IsOutOfBounds(atkDest)) return;
        // Do nothing if destination tile is empty
        if(board.GetCard(atkDest) == null) return;
        
        // Deal damage
        UnitCard target = board.GetCard(atkDest);
        if(card.team != target.team) {
            // animation
            if(mainDuel) {
                float animDuration = 0.3f;
                IEnumerator anim = DuelManager.Instance.AM.CardAttack(
                    card.CardInteractableRef.transform, 
                    atk.direction,
                    animDuration
                );
                QueueableAnimation qa = new QueueableAnimation(anim, animDuration);
                DuelManager.Instance.AM.QueueAnimation(qa);
            }

            DealDamage(target, atk.damage);
        }
    }

    private void DrawCards(Team team, int count) {
        Deck deck;
        if(team == Team.Player) deck = PlayerStatus.Deck;
        else deck = EnemyStatus.Deck;

        for(int i = 0; i < count; i++) {
            int index = Random.Range(0, deck.CardList.Count);
            UnitCard c = ScriptableObject.Instantiate(deck.CardList[index]);
            c.team = team;
            if(mainDuel) {
                DuelEvents.Instance.DrawCard(c, team); // TODO double check
            }
        }
    }

    public void DealDamage(UnitCard card, int damage)
    {
        // TODO activate on hit abilites

        card.Health -= damage;
        if (card.Health <= 0)
        {
            board.RemoveCard(card.pos);

            if(mainDuel) {
                IEnumerator ie = DuelManager.Instance.AM.CardDeath(card.CardInteractableRef);
                QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
                DuelManager.Instance.AM.QueueAnimation(qa);
            }
        }
    }

    private void EndTurn(Team team) {
        // End turn
        Team oppositeTeam;
        CharStatus oppositeStatus;
        if(team == Team.Player) {
            oppositeTeam = Team.Enemy;
            oppositeStatus = EnemyStatus;
        }
        else {
            oppositeTeam = Team.Player;
            oppositeStatus = PlayerStatus;
        }
        // TODO increase mana cap
        oppositeStatus.ResetMana();
        board.RenewMovement(oppositeTeam);
        DrawCards(oppositeTeam, 1);
    }
}
