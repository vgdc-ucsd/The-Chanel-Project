using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Spell Card", menuName = "Cards/SpellBomb")]
public class SpellBomb : SpellCard , ISpellTypeTile
{
    int damage = 1;
    int area = 2;
    public bool CastSpell(DuelInstance duel, BoardCoords pos)
    {
        StartCast(duel, pos);

        List<UnitCard> damagedCards = duel.DuelBoard.GetCardsInSquare(pos, area);
        foreach(UnitCard card in damagedCards)
        {
            duel.DealDamage(card, damage, true);
            AnimationManager.Instance.UpdateCardInfoAnimation(duel, card);
        }

        FinishCast(duel);
        return true;

    }
}