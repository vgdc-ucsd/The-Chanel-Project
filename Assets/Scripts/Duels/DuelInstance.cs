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
    //private int turnNumber = 1;

    private Queue<QueueableAnimation> animations;

    public DuelInstance(CharStatus player, CharStatus enemy, Board board) {
        DuelBoard = board;
        PlayerStatus = player;
        EnemyStatus = enemy;
    }

    public DuelInstance Clone() {
        return new DuelInstance(PlayerStatus.Clone(), EnemyStatus.Clone(), DuelBoard.Clone());
    }

    public Queue<QueueableAnimation> ProcessBoard(Team team) {
        // the team is whoever just activated end turn

        // clear old animations
        animations = new Queue<QueueableAnimation>();

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
        return animations;
    }

    public Queue<QueueableAnimation> DrawStartingCards() {
        animations = new Queue<QueueableAnimation>();

        // Player cards
        DrawCards(Team.Player, DuelManager.Instance.Settings.Player.StartingCards);

        // Enemy Cards
        if(DuelManager.Instance.Settings.SameSettingsForBothPlayers) {
            DrawCards(Team.Enemy, DuelManager.Instance.Settings.Player.StartingCards);    
        }
        else DrawCards(Team.Enemy, DuelManager.Instance.Settings.Enemy.StartingCards);
        
        return animations;
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
            animations.Enqueue(AttackAnimation(card, atk));
            
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
        Deck deck;
        CharStatus status;

        if(team == Team.Player) {
            deck = PlayerStatus.Deck;
            status = PlayerStatus;
        }
        else {
            deck = EnemyStatus.Deck;
            status = EnemyStatus;
        }

        List<Card> drawnCards = new List<Card>();
        for(int i = 0; i < count; i++) {
            // pick a random card, TODO keep track of how many cards are left in deck
            int index = Random.Range(0, deck.CardList.Count);
            Card c = ScriptableObject.Instantiate(deck.CardList[index]);
            c.CurrentTeam = team;
            status.AddCard(c);
            drawnCards.Add(c);
        }

        animations.Enqueue(OrganizeCardsAnimation(drawnCards));
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
            animations.Enqueue(DeathAnimation(target));
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
        // TODO increase mana cap
        oppositeStatus.ResetMana();
        DuelBoard.RenewMovement(oppositeTeam);
        DrawCards(oppositeTeam, 1);
    }

    // TODO move
    private QueueableAnimation AttackAnimation(UnitCard card, Attack atk) {
        float animDuration = 0.3f;
        IEnumerator anim = DuelManager.Instance.AM.CardAttack(
            card.CardInteractableRef.transform, 
            atk.direction,
            animDuration
        );
        QueueableAnimation qa = new QueueableAnimation(anim, animDuration);
        return qa;
    }

    private QueueableAnimation DeathAnimation(UnitCard card) {
        IEnumerator ie = DuelManager.Instance.AM.CardDeath(card.CardInteractableRef);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        return qa;
    }

    private QueueableAnimation OrganizeCardsAnimation(List<Card> cards) {
        IEnumerator ie = DuelManager.Instance.AM.OrganizeCardsAnimation(cards);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        return qa;
    }

    public void UpdateCardInfo(UnitCard c) {
        // queue animation
        if(c.CardInteractableRef != null) {c.CardInteractableRef.UpdateCardInfo();}
    }
}
