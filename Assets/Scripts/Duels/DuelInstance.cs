using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public Team Winner = Team.Neutral;
    [HideInInspector] public bool iteration = true;

    BossData boss;
    public int currBossStage = -1;
    bool triggerNextStage = false;
    bool interrupt = false;

    public DuelInstance(CharStatus player, CharStatus enemy, Board board, BossData boss, int bossStage)
    {
        DuelBoard = board;
        PlayerStatus = player;
        EnemyStatus = enemy;
        Animations = new Queue<QueueableAnimation>();
        Winner = Team.Neutral;
        this.boss = boss;
        this.currBossStage = bossStage;
    }

    public DuelInstance Clone()
    {
        return new DuelInstance(PlayerStatus.Clone(), EnemyStatus.Clone(), DuelBoard.Clone(), boss, currBossStage);
    }

    public void ProcessBoard(Team team)
    {
        ActivationInfo info = new ActivationInfo(this);
        // the team is whoever just activated end turn
        foreach (UnitCard c in DuelBoard.GetAllCards())
        {
            c.Processed = false;
        }
        // Process all cards
        for (int i = 0; i < DuelBoard.Cols; i++)
        {
            for (int j = 0; j < DuelBoard.Rows; j++)
            {
                BoardCoords pos = new BoardCoords(i, j);
                if (DuelBoard.IsOccupied(pos))
                {
                    ProcessCard(DuelBoard.GetCard(pos), team);
                }
                if (interrupt) break;
            }
            if (interrupt) break;
        }

        interrupt = false;
        if (triggerNextStage)
        {
            if (DuelManager.Instance.CurrentEncounter.Settings.SameSettingsForBothPlayers)
            {
                EnemyStatus.Health = (int)(DuelManager.Instance.CurrentEncounter.Settings.Player.MaxHealth * 1.5);
            }
            else
            {
                EnemyStatus.Health = (int)(DuelManager.Instance.CurrentEncounter.Settings.Enemy.MaxHealth * 1.5);
            }

            if (boss.clearBoard)
            {
                foreach (UnitCard c in DuelBoard.GetCardsOfTeam(Team.Player))
                {
                    DealDamage(c, 9999);
                }
            }

            if (boss.buffCards)
            {
                foreach (UnitCard c in DuelBoard.GetCardsOfTeam(Team.Enemy))
                {
                    AnimationManager.Instance.AbilityActivateAnimation(this, c);
                    ++c.Health;
                    AnimationManager.Instance.DamageCardAnimation(this, c, Color.yellow, -1);
                }
                foreach (Card c in EnemyStatus.Deck.CardList)
                {
                    if (c is UnitCard uc)
                    {
                        uc.Health++;
                    }
                }
            }

            triggerNextStage = false;
        }
        EndTurn(team, info.Duel);
    }

    public Queue<QueueableAnimation> DrawStartingCards()
    {
        Animations = new Queue<QueueableAnimation>();

        // Player cards
        DrawCards(Team.Player, DuelManager.Instance.Settings.Player.StartingCards);

        // Enemy Cards
        if (DuelManager.Instance.Settings.SameSettingsForBothPlayers)
        {
            DrawCards(Team.Enemy, DuelManager.Instance.Settings.Player.StartingCards);
        }
        else DrawCards(Team.Enemy, DuelManager.Instance.Settings.Enemy.StartingCards);

        return Animations;
    }

    private void ProcessCard(UnitCard card, Team team)
    {
        if (card.Processed) return;
        card.Processed = true;
        // Card glow off animation
        IEnumerator ie = AnimationManager.Instance.CardCantMove(card);
        QueueableAnimation qa = new QueueableAnimation(ie, 0f);
        Animations.Enqueue(qa);

        // Cards only take actions on their turn
        if (card.CurrentTeam == team)
        {

            // Activate abilities
            ActivationInfo info = new ActivationInfo(this);
            foreach (Ability a in card.Abilities)
            {
                // Only activate if the activation condition is OnProcess
                if (a.Condition == ActivationCondition.OnProcess)
                {
                    a.Activate(card, info);
                }
            }

            if (card.drawStatus != DrawStatus.InPlay) return;

            // Attack
            if (card.CanAttack)
            {
                List<Attack> queuedCharAttacks = new List<Attack>();
                List<UnitCard> damagedCards = new List<UnitCard>();
                bool attackLanded = false;
                BoardCoords lastPos = card.Pos;
                for (int i = card.Attacks.Count - 1; i >= 0; i--)
                {
                    if (card.Pos != lastPos) queuedCharAttacks.Clear(); // Merlin fix
                    lastPos = card.Pos;

                    BoardCoords atkDest = card.Pos + new BoardCoords(card.Attacks[i].direction);

                    if ((DuelBoard.BeyondEnemyEdge(atkDest) && team == Team.Player) ||
                         DuelBoard.BeyondPlayerEdge(atkDest) && team == Team.Enemy)
                    {
                        queuedCharAttacks.Add(card.Attacks[i]);
                        continue;
                    }
                    UnitCard target = ProcessAttack(card, card.Attacks[i]);
                    if (target != null) attackLanded = true;
                    if (target != null && target.Health > 0) damagedCards.Add(target);
                }

                if (card.Pos != lastPos) queuedCharAttacks.Clear(); // Merlin fix

                if (queuedCharAttacks.Count != 0)
                {
                    attackLanded = true;
                    Attack maxDmgAtk = queuedCharAttacks[0];
                    foreach (Attack atk in queuedCharAttacks)
                    {
                        if (atk.damage > maxDmgAtk.damage) maxDmgAtk = atk;
                    }
                    Team winner = GetStatus(CharStatus.OppositeTeam(team)).TakeDamage(maxDmgAtk.damage);
                    AnimationManager.Instance.AttackAnimation(this, card, maxDmgAtk);
                    AnimationManager.Instance.DamagePlayerAnimation(this, GetStatus(CharStatus.OppositeTeam(team)), maxDmgAtk.damage);
                    if (winner != Team.Neutral)
                    {
                        interrupt = interrupt || TryWin(winner);
                    }

                }
                if (attackLanded)
                {
                    info.DamagedCards = damagedCards;
                    for (int i = card.Abilities.Count - 1; i >= 0; i--)
                    {
                        Ability a = card.Abilities[i];
                        if (a.Condition == ActivationCondition.OnFinishAttack) a.Activate(card, info);
                    }
                }


            }

        }
    }

    private bool TryWin(Team winner)
    {
        if (triggerNextStage) return false;
        if (winner == Team.Enemy || boss == null || boss.stages <= currBossStage)
        {
            Winner = winner;
            return true;
        }
        else
        {
            // boss out of health but has stages remaining
            currBossStage++;
            // cool boss effects
            triggerNextStage = true;
            return true;
        }
    }

    private UnitCard ProcessAttack(UnitCard card, Attack atk)
    {
        BoardCoords atkDest = card.Pos + new BoardCoords(atk.direction);

        // Do nothing if attack is out of bounds
        if (DuelBoard.IsOutOfBounds(atkDest)) return null;

        // Do nothing if destination tile is empty
        if (DuelBoard.GetCard(atkDest) == null) return null;

        // Deal damage
        UnitCard target = DuelBoard.GetCard(atkDest);
        if (card.CurrentTeam != target.CurrentTeam)
        {
            // Animation
            AnimationManager.Instance.AttackAnimation(this, card, atk);

            // Abilities
            ActivationInfo info = new ActivationInfo(this);
            info.TargetCard = target;
            info.TotalDamage = atk.damage;

            for (int i = card.Abilities.Count - 1; i >= 0; i--)
            {
                if (card.Abilities[i].Condition == ActivationCondition.OnAttack) card.Abilities[i].Activate(card, info);
            }

            info.OverkillDamage = DealDamage(target, atk.damage);
            for (int i = card.Abilities.Count - 1; i >= 0; i--)
            {
                if (card.Abilities[i].Condition == ActivationCondition.OnDealDamage) card.Abilities[i].Activate(card, info);
            }

            for (int i = target.Abilities.Count - 1; i >= 0; i--)
            {
                if (target.Abilities[i].Condition == ActivationCondition.OnAttacksHitMe) target.Abilities[i].Activate(card, info);
            }
            return target;
        }
        return null;
    }



    public void DrawCardWithMana(Team team)
    {
        CharStatus status = GetStatus(team);
        Deck deck = status.Deck;

        if (deck.DrawPileIsEmpty())
        {
            Debug.Log("Cannot draw card, no cards remaining");
            return;
        }
        if (!status.CanUseMana(DuelManager.Instance.Settings.DrawCardManaCost))
        {
            Debug.Log("Cannot draw card, not enough mana");
            return;
        }

        status.UseMana(DuelManager.Instance.Settings.DrawCardManaCost);
        DrawCards(team, 1, true);
    }

    private void DrawCards(Team team, int count, bool immediate = false)
    {
        CharStatus status = GetStatus(team);
        Deck deck = status.Deck;


        List<Card> drawnCards = new List<Card>();

        if (deck.DrawPileIsEmpty())
        {
            deck.Refresh();
        }

        for (int i = 0; i < count; i++)
        {
            // pick a random card, TODO keep track of how many cards are left in deck


            Card drawnCard = deck.RandomAvailableCard();


            if (drawnCard == null) break;

            drawnCard.drawStatus = DrawStatus.InPlay;
            PlayerStatus.drawPileCards = PlayerStatus.Deck.DrawPile();
            deck.numAvailableCards--;
            // Debug.Log($"Team: {team}, cards: {deck.numAvailableCards}");
            Card c = drawnCard.Clone();
            c.CurrentTeam = team;


            status.AddCard(c);
            drawnCards.Add(c);

            if (deck.DrawPileIsEmpty())
            {
                deck.Refresh();
            }

        }

        AnimationManager.Instance.DrawCardsAnimation(this, drawnCards, team);
    }

    public int DealDamage(UnitCard target, int damage, bool immediate = false)
    {
        int overkillDamage = 0;
        target.TakeDamage(this, damage);

        // On card death
        if (target.Health <= 0)
        {
            overkillDamage = -1 * target.Health;
            target.Health = 0;
            DuelBoard.RemoveCard(target.Pos);
            AnimationManager.Instance.DeathAnimation(this, target);
        }

        return overkillDamage;
    }

    private void EndTurn(Team team, DuelInstance duel)
    {
        ActivationInfo info = new ActivationInfo(this);
        //Glow off
        //HandInterface.Instance.EndTurnGlow();
        foreach (UnitCard card in DuelBoard.GetCardsOfTeam(team))
        {
            for (int i = card.Abilities.Count - 1; i >= 0; i--)
            {
                Ability ability = card.Abilities[i];
                if (ability.Condition == ActivationCondition.OnEndTurn)
                {
                    ability.Activate(card, info);
                }
            }
        }

        // End turn
        Team oppositeTeam;
        CharStatus oppositeStatus;
        if (team == Team.Player)
        {
            oppositeTeam = Team.Enemy;
            oppositeStatus = EnemyStatus;
        }
        else
        {
            oppositeTeam = Team.Player;
            oppositeStatus = PlayerStatus;
        }
        // gain 1 mana capacity every turn until it reaches the max then it caps out;
        oppositeStatus.GiveMana();
        DuelBoard.RenewMovement(oppositeTeam, info.Duel);


        foreach (UnitCard card in DuelBoard.GetCardsOfTeam(oppositeTeam))
        {
            for (int i = card.Abilities.Count - 1; i >= 0; i--)
            {
                if (i < card.Abilities.Count)
                {
                    Ability ability = card.Abilities[i];
                    if (ability.Condition == ActivationCondition.OnBeginTurn)
                    {
                        ability.Activate(card, info);
                    }
                }
            }
        }
        foreach (UnitCard card in DuelBoard.GetCardsOfTeam(team))
        {
            for (int i = card.Abilities.Count - 1; i >= 0; i--)
            {
                Ability ability = card.Abilities[i];
                if (ability.Condition == ActivationCondition.OnBeginOppositeTurn)
                {
                    ability.Activate(card, info);
                }
            }
        }

        DrawCards(oppositeTeam, 1);
    }

    public CharStatus GetStatus(Team team)
    {
        if (team == Team.Player) return PlayerStatus;
        else return EnemyStatus;
    }
}
