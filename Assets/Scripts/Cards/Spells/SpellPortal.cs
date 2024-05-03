using System.Collections.Generic;
using UnityEngine;

public class SpellPortal : Spell, ISpellTypeTwoUnits
{
    public void CastSpell(UnitCard unit1, UnitCard unit2)
    {
        Board board = DuelManager.Instance.MainDuel.DuelBoard;
        board.RemoveCard(unit1.Pos);
        board.RemoveCard(unit2.Pos);
        BoardCoords oldPos = unit2.Pos;
        //board.MoveCard(unit2, unit1.Pos, true);
        //board.MoveCard(unit1, oldPos, true, true);

    }
}