using System.Collections.Generic;
using UnityEngine;

public class SpellPortal : Spell, ISpellTypeTwoUnits
{
    public void CastSpell(UnitCard unit1, UnitCard unit2)
    {
        Board board = DuelManager.Instance.CurrentBoard;
        board.RemoveCard(unit1.pos);
        board.RemoveCard(unit2.pos);
        BoardCoords oldPos = unit2.pos;
        board.MoveCard(unit2, unit1.pos, true);
        board.MoveCard(unit1, oldPos, true, true);

    }
}