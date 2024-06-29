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


    const float MOVE_BASELINE_SCORE = 30;
    // DEFENDING ACTIONS
    const float D_ATTACKING_SCORE = 80; // score bonus for landing each attack 
    const float D_ATTACKED_SCORE = -40; // score malus for potentially taking each attack


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
            MoveType moveType = MoveType.Defending;
            // TEMP, will randomly decide to whether make an offensive
            // or defensive action for each action

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
            moves[dir] = EvaluateCardPosition(move.card, move.card.Pos + MoveUnit.dirVectors[dir], moveType, dir == MoveUnit.Direction.None);
        }
        if (moves.Count == 0) return; // should never happen

        Debug.Log(move.card.Name + " possible moves");
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
            placements[tile] = EvaluateCardPosition(place.card, tile, moveType);
        }
        if (placements.Count == 0) return;

        Debug.Log(place.card.Name + " possible placements");
        Debug.Log(placements.ToLineSeparatedString()) ;
        //Debug.Break();

        // find placement with highest evaluation
        BoardCoords bestPlacement = placements.First().Key;
        foreach (KeyValuePair<BoardCoords, float> kvp in placements)
        {
            if (kvp.Value > placements[bestPlacement])
                bestPlacement = kvp.Key;
        }
        // make the move
        duel.DuelBoard.PlayCard(place.card, bestPlacement, duel.GetStatus(team), duel);
        predictedKilledCards.AddRange(GetKilledCards(place.card, place.card.Pos));
    }

    // TODO split into move and place
    private float EvaluateCardPosition(UnitCard card, BoardCoords pos, MoveType moveType, bool stay = false)
    {
        // reject illegal moves; staying in place is never an illegal move
        if (!stay)
        {
            if (duel.DuelBoard.IsOutOfBounds(pos) || duel.DuelBoard.IsOccupied(pos))
                return int.MinValue;
        }
        float score = 0;
        if (stay) score += MOVE_BASELINE_SCORE; // default bias for staying in place

        List<UnitCard> killedUnits = new List<UnitCard>();

        if (moveType == MoveType.Defending)
        {
            foreach (UnitCard atkTarget in GetOutgoingAttacks(card,pos))
            {
                score += D_ATTACKING_SCORE;
                if (atkTarget.Health <= card.BaseDamage) killedUnits.Add(atkTarget);
            }

            foreach (UnitCard attacker in GetIncomingAttacks(card,pos))
            {
                if (!killedUnits.Contains(attacker)) // nullify attacker score if it would be killed
                    score += D_ATTACKED_SCORE;
            }
            // TODO
            // prioritize targets that are close to home row
            
        }
        else if (moveType == MoveType.Attacking)
        {
            // TODO
            // bonus for forward movement and attacking player
            // less bonus for attacking cards, more focus on pushing forward
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



