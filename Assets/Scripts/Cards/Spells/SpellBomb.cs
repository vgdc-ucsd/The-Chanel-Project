using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SpellBomb : SpellCard , ISpellTypeTile
{
    int damage = 2;
    int area = 2;
    public bool CastSpell(DuelInstance duel, BoardCoords pos)
    {
        List<UnitCard> damagedCards = duel.DuelBoard.GetCardsInSquare(pos, area);
        Debug.Log(damagedCards.ToCommaSeparatedString());
        foreach(UnitCard card in damagedCards)
        {
            duel.DealDamage(card, damage, true);
        }
        return true;
    }
}