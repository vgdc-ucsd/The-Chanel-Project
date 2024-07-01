using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class EnemyAI
{

    private enum MoveType
    {
        Attacking, Defending
    }

    private class AIAction
    {

    }

    private class PlaceUnit : AIAction
    {
        public PlaceUnit(UnitCard card)
        {
            this.card = card;
        }

        public UnitCard card;
    }

    private class MoveUnit : AIAction
    {
        public enum Direction
        {
            Left, Right, Up, Down, None
        }
        public static Dictionary<Direction, BoardCoords> dirVectors =
            new Dictionary<Direction, BoardCoords>()
            {
                {Direction.Left, new BoardCoords(-1,0)},
                {Direction.Right, new BoardCoords(1,0)},
                {Direction.Up, new BoardCoords(0,1)},
                {Direction.Down, new BoardCoords(0,-1)},
                {Direction.None, new BoardCoords(0,0)}
            };

        public MoveUnit(UnitCard card)
        {
            this.card = card;
        }
        public UnitCard card;
    }

    const int AI_HOME_ROW = 3;
    const int PLAYER_HOME_ROW = 0;

    const float AGGRESSION_BASELINE = 0.75f; // base chance for an action to be attacking
    const float THREAT_DEFENSE_RESPONSE = 0.15f; // added chance for defensive move per pt of threat
    // threat is calculated as: every enemy card on home row is worth 3 pts,
    // every enemy card 1 row away from home row worth 1 pt

    // DEFENDING ACTIONS
    const float D_STAY_SCORE = 0;
    const float D_PUSH_SCORE = 5;       // score for moving forward
    const float D_ATTACKING_SCORE = 80; // score bonus for landing each attack 
    const float D_ATTACKED_SCORE = -40; // score malus for potentially taking each attack

    // ATTACKING ACTIONS
    const float A_STAY_SCORE = -10; // discourage staying in place
    const float A_PUSH_SCORE = 30;       // score for moving forward
    const float A_ATTACKING_SCORE = 25; // score bonus for landing each attack 
    const float A_ATTACKED_SCORE = -20; // score malus for potentially taking each attack



    Team team = Team.Enemy;
    DuelInstance duel;
    private List<AIAction> possibleActions;
    private List<UnitCard> predictedKilledCards;

    public void PlayTurn(DuelInstance duel)
    {
        predictedKilledCards = new List<UnitCard>();
        this.duel = duel;
        AddPossibleActions();

        foreach (AIAction action in possibleActions)
        {
            int threat = 0;
            foreach (UnitCard uc in duel.DuelBoard.GetCardsInRow(AI_HOME_ROW))
            {
                if (uc.CurrentTeam == CharStatus.OppositeTeam(team)) threat += 3;
            }
            foreach (UnitCard uc in duel.DuelBoard.GetCardsInRow(AI_HOME_ROW - 1))
            {
                if (uc.CurrentTeam == CharStatus.OppositeTeam(team)) threat += 1;
            }

            float offenseChance = AGGRESSION_BASELINE - THREAT_DEFENSE_RESPONSE * threat;
            Debug.Log(offenseChance);
            if (action is MoveUnit mov)
            {
                if (mov.card.Pos.y < AI_HOME_ROW - 1)
                {
                    offenseChance += 0.5f; // high chance of attacking move if card is already near player home
                }
                if (mov.card.Pos.y == AI_HOME_ROW && threat >= 3)
                {
                    offenseChance -= 0.5f; // high chance of defending if already near home and there are threatening cards
                }
            }
            

            offenseChance = Mathf.Clamp01(offenseChance);

            MoveType moveType = MoveType.Defending;
            if (Random.value < offenseChance)
            {
                moveType = MoveType.Attacking;
            }

            if (action is MoveUnit m)
                TryMove(m, moveType);
            else if (action is PlaceUnit p)
                TryPlace(p, moveType);
        }

        // use up remaining mana to draw cards
        while (duel.GetStatus(team).CanDrawCard())
        {
            duel.DrawCardWithMana(team);
        }


        UpdateUnitCardInteractableRefs(duel);

        //UnityEngine.Debug.Log("Final Move Score " + bestMove.Score());

        DuelManager.Instance.EnemyMove(duel);
    }

    // create a list of all possible actions (move, play card) that will be evaluated
    private void AddPossibleActions()
    {
        possibleActions = new List<AIAction>();
        foreach (UnitCard uc in duel.DuelBoard.GetCardsOfTeam(team)) {
            if (uc.CanMove)
            {
                possibleActions.Add(new MoveUnit(uc));
            }
        }
        foreach (Card card in duel.GetStatus(team).Cards)
        {
            if (card is UnitCard uc)
            {
                possibleActions.Add(new PlaceUnit(uc));
            }
        }
        possibleActions.Shuffle();

    }

    private void TryMove(MoveUnit move, MoveType moveType)
    {
        // map possible places that the card can end up with its evaluation
        Dictionary<MoveUnit.Direction, float> moves = new Dictionary<MoveUnit.Direction, float>();
        List<MoveUnit.Direction> directions = new List<MoveUnit.Direction>(MoveUnit.dirVectors.Keys);
        directions.Shuffle();
        foreach (MoveUnit.Direction dir in MoveUnit.dirVectors.Keys)
        {
            moves[dir] = EvaluateMovePosition(move.card, move.card.Pos + MoveUnit.dirVectors[dir], moveType, dir == MoveUnit.Direction.None);
        }
        if (moves.Count == 0) return; // should never happen

        Debug.Log(move.card.Name + " possible moves, " + moveType);
        Debug.Log(moves.ToLineSeparatedString());
        //Debug.Break();


        // find direction with highest evaluation
        MoveUnit.Direction bestMove = moves.First().Key;
        foreach (KeyValuePair<MoveUnit.Direction, float> kvp in moves)
        {
            if (kvp.Value > moves[bestMove]) 
                bestMove = kvp.Key;
        }
        // make the move
        duel.DuelBoard.MoveCard(move.card, move.card.Pos + MoveUnit.dirVectors[bestMove], duel);
        predictedKilledCards.AddRange(GetKilledCards(move.card, move.card.Pos));
    }

    private void TryPlace(PlaceUnit place, MoveType moveType)
    {
        if (duel.GetStatus(team).Mana < place.card.ManaCost) return;
        Dictionary<BoardCoords, float> placements = new Dictionary<BoardCoords, float>();
        List<BoardCoords> legalTiles = GetLegalTiles(duel.DuelBoard);
        legalTiles.Shuffle();
        foreach (BoardCoords tile in legalTiles)
        {
            if (DuelManager.Instance.Settings.SummoningSickness)
                placements[tile] = EvaluatePlacePosition(place.card, tile, moveType);
            else
                placements[tile] = EvaluateMovePosition(place.card, tile, moveType);
        }
        if (placements.Count == 0) return;

        Debug.Log(place.card.Name + " possible placements, " + moveType);
        Debug.Log(placements.ToLineSeparatedString()) ;
        //Debug.Break();

        // find placement with highest evaluation
        BoardCoords bestPlacement = placements.First().Key;
        foreach (KeyValuePair<BoardCoords, float> kvp in placements)
        {
            if (kvp.Value > placements[bestPlacement])
                bestPlacement = kvp.Key;
        }
        if (placements[bestPlacement] < 0) return;
        // make the move
        duel.DuelBoard.PlayCard(place.card, bestPlacement, duel.GetStatus(team), duel);
        predictedKilledCards.AddRange(GetKilledCards(place.card, place.card.Pos));
    }

    private float EvaluateMovePosition(UnitCard card, BoardCoords pos, MoveType moveType, bool stay = false)
    {
        // reject illegal moves; staying in place is never an illegal move
        if (!stay)
        {
            if (duel.DuelBoard.IsOutOfBounds(pos) || duel.DuelBoard.IsOccupied(pos))
                return int.MinValue;
        }
        float score = 0;
        

        List<UnitCard> killedUnits = new List<UnitCard>();

        if (moveType == MoveType.Defending)
        {
            if (stay) score += D_STAY_SCORE; // default bias for staying in place

            switch (pos.y)                  // slight bias for forward movement, if all else are equal
            {
                case AI_HOME_ROW:
                    score += 0;
                    break;
                case AI_HOME_ROW - 1:
                    score += D_PUSH_SCORE;
                    break;
                case AI_HOME_ROW - 2:
                    score += D_PUSH_SCORE * 2;
                    break;
                case AI_HOME_ROW - 3:
                    score += D_PUSH_SCORE * 3;
                    break;
                // I SWEAR THIS IS NOT BAD CODE, this is made to be adjustable
            }

            foreach (UnitCard atkTarget in GetOutgoingAttacks(card,pos))
            {
                // prioritize threatening targets
                float multiplier = 1;
                if (atkTarget.Pos.y == AI_HOME_ROW) multiplier = 3;
                else if (atkTarget.Pos.y == AI_HOME_ROW - 1) multiplier = 1.8f;

                score += D_ATTACKING_SCORE * multiplier;
                if (atkTarget.Health <= card.BaseDamage) killedUnits.Add(atkTarget);
            }

            foreach (UnitCard attacker in GetIncomingAttacks(card,pos))
            {
                if (!killedUnits.Contains(attacker)) // nullify attacker score if it would be killed
                    score += D_ATTACKED_SCORE;
            }
            
        }
        else if (moveType == MoveType.Attacking)
        {
            // TODO
            // bonus for forward movement and attacking player
            // less bonus for attacking cards, more focus on pushing forward

            if (stay) score += A_STAY_SCORE;

            // more score when the move ends up closer to the player
            switch (pos.y)
            {
                case AI_HOME_ROW:
                    score += A_PUSH_SCORE * -1;
                    break;
                case AI_HOME_ROW - 1:
                    score += A_PUSH_SCORE * 0;
                    break;
                case AI_HOME_ROW - 2:
                    score += A_PUSH_SCORE * 1;
                    break;
                case AI_HOME_ROW - 3:
                    score += A_PUSH_SCORE * 2 + 20;
                    break;
            }

            foreach (UnitCard atkTarget in GetOutgoingAttacks(card, pos))
            {
                score += A_ATTACKING_SCORE;
                if (atkTarget.Health <= card.BaseDamage) killedUnits.Add(atkTarget);
            }

            foreach (UnitCard attacker in GetIncomingAttacks(card, pos))
            {
                if (!killedUnits.Contains(attacker)) // nullify attacker score if it would be killed
                    score += A_ATTACKED_SCORE;
            }

        }


        return score;
    }

    // for summoning sickness testing only
    private float EvaluatePlacePosition(UnitCard card, BoardCoords pos, MoveType moveType)
    {
        // reject illegal moves; staying in place is never an illegal move

        if (duel.DuelBoard.IsOutOfBounds(pos) || duel.DuelBoard.IsOccupied(pos))
            return int.MinValue;

        float score = 0;
        if (moveType == MoveType.Defending)
        {
            foreach (UnitCard attacker in GetIncomingAttacks(card, pos))
            {
                score += D_ATTACKED_SCORE;
            }

        }
        else if (moveType == MoveType.Attacking)
        {
            // TODO
            // bonus for forward movement and attacking player
            // less bonus for attacking cards, more focus on pushing forward

            // more score when the move ends up closer to the player
            switch (pos.y)
            {
                case AI_HOME_ROW:
                    score += -30;
                    break;
                case AI_HOME_ROW - 1:
                    break;
                case AI_HOME_ROW - 2:
                    score += 30;
                    break;
                case AI_HOME_ROW - 3:
                    score += 80;
                    break;
            }

            foreach (UnitCard attacker in GetIncomingAttacks(card, pos))
            {
                score += A_ATTACKED_SCORE;
            }

        }


        return score;
    }


    private List<UnitCard> GetOutgoingAttacks(UnitCard card, BoardCoords pos)
    {
        List<UnitCard> result = new List<UnitCard>();
        foreach (Attack atk in card.Attacks)
        {
            UnitCard target = duel.DuelBoard.GetCard(pos + new BoardCoords(atk.direction));
            if (target != null && target.CurrentTeam == CharStatus.OppositeTeam(team)
                && !predictedKilledCards.Contains(target)) // ignore card if it will already be killed by another card
            {
                result.Add(target);
            }
        }
        return result;
    }

    private List<UnitCard> GetIncomingAttacks(UnitCard card, BoardCoords pos)
    {
        List<UnitCard> result = new List<UnitCard>();
        foreach (UnitCard attacker in duel.DuelBoard.GetCardsInSquare(pos,2))
        {
            if (predictedKilledCards.Contains(attacker)) continue;
            foreach (Attack atk in attacker.Attacks)
            {
                if (attacker.CurrentTeam == CharStatus.OppositeTeam(team) 
                    && attacker.Pos + new BoardCoords(atk.direction) == pos)
                    result.Add(attacker);
            }
        }
        return result;
    }

    private List<UnitCard> GetKilledCards(UnitCard card, BoardCoords pos)
    {
        List<UnitCard> result = new List<UnitCard>();
        foreach (Attack atk in card.Attacks)
        {
            UnitCard target = duel.DuelBoard.GetCard(pos + new BoardCoords(atk.direction));
            if (target != null && target.CurrentTeam == CharStatus.OppositeTeam(team)
                && !predictedKilledCards.Contains(target)) // ignore card if it will already be killed by another card
            {
                if (target.Health <= atk.damage)
                    result.Add(target);
            }
        }
        return result;
    }

    // copied from MCTS
    private List<BoardCoords> GetLegalTiles(Board b)
    {
        List<BoardCoords> legalTiles = new List<BoardCoords>();

        for (int i = 0; i < b.Rows; i++)
        {
            for (int j = 0; j < b.Cols; j++)
            {
                BoardCoords pos = BoardCoords.FromRowCol(new Vector2Int(i, j));
                if (!b.IsOccupied(pos))
                {
                    if (DuelManager.Instance.Settings.RestrictPlacement && i < 2)
                    { // can't place in two rows closest to player
                        legalTiles.Add(pos);
                    }
                }
            }
        }

        return legalTiles;
    }

    private void UpdateUnitCardInteractableRefs(DuelInstance move)
    {
        // Loops over every card and updates its cardinteractable to reference itself

        foreach (UnitCard uc in move.DuelBoard.CardSlots)
        {
            if (uc != null && uc.CardInteractableRef != null)
            {
                UnitCardInteractable uci = uc.UnitCardInteractableRef;
                uci.card = uc;
            }
        }
    }

    
}



