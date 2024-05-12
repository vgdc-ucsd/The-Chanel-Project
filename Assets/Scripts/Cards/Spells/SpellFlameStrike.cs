using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Flame Strike Spell Card", menuName = "Cards/SpellFlameStrike")]
public class SpellFlameStrike : SpellCard , ISpellTypeTile
{
    int damage = 2;
    public bool CastSpell(DuelInstance duel, BoardCoords pos)
    {
        List<UnitCard> damagedCards = duel.DuelBoard.GetCardsInRow(pos.y);
        foreach(UnitCard card in damagedCards)
        {
            duel.DealDamage(card, damage, true);
            AnimationManager.Instance.UpdateCardInfoAnimation(duel,card);
        }



        FinishCast(duel);
        return true;

    }
}