using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "Portal Spell Card", menuName = "Cards/SpellPortal")]
public class SpellPortal : SpellCard, ISpellTypeUnit
{
    public bool CastSpell(DuelInstance duel, UnitCard card)
    {

        Board board = duel.DuelBoard;
        BoardCoords targetPos;
        if (CurrentTeam == Team.Player) targetPos = new BoardCoords(card.Pos.x, 3);
        else targetPos = new BoardCoords(card.Pos.x, 0);

        if (targetPos == card.Pos) return false;

        if (board.IsOccupied(targetPos))
        {
            UnitCard unit2 = board.GetCard(targetPos);

            BoardCoords oldPos = unit2.Pos;
            board.TeleportCard(unit2, card.Pos, duel);
            board.TeleportCard(card, oldPos, duel);
        }

        else
        {
            board.TeleportCard(card, targetPos, duel);
        }

        FinishCast(duel);
        return true;
    }

    
}