using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Tsunami Spell Card", menuName = "Cards/SpellTsunami")]
public class SpellTsunami : SpellCard , ISpellTypeTile
{
    public bool CastSpell(DuelInstance duel, BoardCoords pos)
    {
        BoardCoords push;
        if (CurrentTeam == Team.Player) push = new BoardCoords(0, 1);
        else push = new BoardCoords(0, -1);
        Board board = duel.DuelBoard;
        if (board.IsOutOfBounds(pos + push)) return false;
        if (board.GetCardsInRow(pos.y).Count < 1) return false;
        List<UnitCard> targets = new List<UnitCard>();
        foreach (UnitCard uc in board.GetCardsInRow(pos.y)) {
            if (uc.CurrentTeam != CurrentTeam && !board.IsOccupied(uc.Pos + push)) {
                targets.Add(uc);
            }
        }
        if (targets.Count < 1) return false;

        StartCast(duel, pos);

        List<UnitCard> pushedCards = new List<UnitCard>();
        foreach (UnitCard c in board.GetCardsInRow(pos.y))
        {
            if (c.CurrentTeam == CurrentTeam || board.IsOccupied(c.Pos + push)) continue;
            board.MoveCard(c, c.Pos + push, duel);

        }

        FinishCast(duel);

        return true;
    }
}