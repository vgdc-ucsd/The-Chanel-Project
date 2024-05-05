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
    public Board DuelBoard;
    public CharStatus PlayerStatus, EnemyStatus;
    public Queue<QueueableAnimation> Animations;

    public DuelInstance(CharStatus player, CharStatus enemy, Board board) {
        DuelBoard = board;
        PlayerStatus = player;
        EnemyStatus = enemy;
        Animations = new Queue<QueueableAnimation>();
    }

    public DuelInstance Clone() {
        return new DuelInstance(PlayerStatus.Clone(), EnemyStatus.Clone(), DuelBoard.Clone());
    }

    public void ProcessBoard(Team team) {
        // the team is whoever just activated end turn

        // clear old animations
        //Animations = new Queue<QueueableAnimation>();

        // Process all cards
        for(int i = 0; i < DuelBoard.Cols; i++) {
            for(int j = 0; j < DuelBoard.Rows; j++) {
                BoardCoords pos = new BoardCoords(i,j);
                if (DuelBoard.IsOccupied(pos)) {
                    ProcessCard(DuelBoard.GetCard(pos), team);
                }
            }
        }

        EndTurn(team);
    }

    public Queue<QueueableAnimation> DrawStartingCards() {
        Animations = new Queue<QueueableAnimation>();

        // Player cards
        DrawCards(Team.Player, DuelManager.Instance.Settings.Player.StartingCards);

        // Enemy Cards
        if(DuelManager.Instance.Settings.SameSettingsForBothPlayers) {
            DrawCards(Team.Enemy, DuelManager.Instance.Settings.Player.StartingCards);    
        }
        else DrawCards(Team.Enemy, DuelManager.Instance.Settings.Enemy.StartingCards);
        
        return Animations;
    }

    private void ProcessCard(UnitCard card, Team team) {
        // Cards only take actions on their turn
        if (card.CurrentTeam == team) {

            // Activate abilities        
            ActivationInfo info = new ActivationInfo(this);
            foreach(Ability a in card.Abilities) {
                // Only activate if the activation condition is OnProcess
                if(a.Condition == ActivationCondition.OnProcess) {
                    a.Activate(card, info);
                }
            }

            // Attack
            foreach(Attack atk in card.Attacks) {
                ProcessAttack(card, atk);
            }
        }
    }

    private void ProcessAttack(UnitCard card, Attack atk) {
        BoardCoords atkDest = card.Pos + new BoardCoords(atk.direction);

        // Attack targeting enemy
        if(DuelBoard.BeyondEnemyEdge(atkDest) && card.CurrentTeam == Team.Player) {
            EnemyStatus.TakeDamage(atk.damage);
            return;
        }

        // Attack targeting player
        if(DuelBoard.BeyondPlayerEdge(atkDest) && card.CurrentTeam == Team.Enemy) {
            PlayerStatus.TakeDamage(atk.damage);
            return;
        }
        
        // Do nothing if attack is out of bounds
        if(DuelBoard.IsOutOfBounds(atkDest)) return;

        // Do nothing if destination tile is empty
        if(DuelBoard.GetCard(atkDest) == null) return;
        
        // Deal damage
        UnitCard target = DuelBoard.GetCard(atkDest);
        if(card.CurrentTeam != target.CurrentTeam) {
            // Animation
            AnimationManager.Instance.AttackAnimation(this, card, atk);
            
            // Abilities
            ActivationInfo info = new ActivationInfo(this);
            info.TotalDamage = atk.damage;
            info.OverkillDamage = DealDamage(target, atk.damage);
            foreach(Ability a in card.Abilities) {
                if(a.Condition == ActivationCondition.OnDealDamage) a.Activate(card, info);
            }
        }
    }

    private void DrawCards(Team team, int count) {
        CharStatus status = GetStatus(team);
        Deck deck = status.Deck;

        List<Card> drawnCards = new List<Card>();
        for(int i = 0; i < count; i++) {
            // pick a random card, TODO keep track of how many cards are left in deck
            int index = Random.Range(0, deck.CardList.Count);
            Card c = ScriptableObject.Instantiate(deck.CardList[index]);
            c.CurrentTeam = team;
            status.AddCard(c);
            drawnCards.Add(c);
        }

        AnimationManager.Instance.OrganizeCardsAnimation(this, drawnCards);
    }

    public int DealDamage(UnitCard target, int damage)
    {
        int overkillDamage = 0;
        target.TakeDamage(this, damage);

        // On card death
        if (target.Health <= 0)
        {
            overkillDamage = -1*target.Health;
            DuelBoard.RemoveCard(target.Pos);
            AnimationManager.Instance.DeathAnimation(this, target);
        }

        return overkillDamage;
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

        // gain 1 mana capacity every turn until it reaches the max then it caps out;
        oppositeStatus.GiveMana();

        DuelBoard.RenewMovement(oppositeTeam);
        DrawCards(oppositeTeam, 1);
    }

    public CharStatus GetStatus(Team team) {
        if(team == Team.Player) return PlayerStatus;
        else return EnemyStatus;
    }
}
