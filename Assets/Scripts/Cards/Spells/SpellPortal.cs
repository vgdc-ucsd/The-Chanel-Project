using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpellPortal : SpellCard, ISpellTypeTwoUnits
{
    public bool CastSpell(DuelInstance duel, UnitCard unit1, UnitCard unit2)
    {
        Board board = duel.DuelBoard;
        board.RemoveCard(unit1.Pos);
        board.RemoveCard(unit2.Pos);
        BoardCoords oldPos = unit2.Pos;
        //board.MoveCard(unit2, unit1.Pos, true);
        //board.MoveCard(unit1, oldPos, true, true);

        return true;
    }

    
}