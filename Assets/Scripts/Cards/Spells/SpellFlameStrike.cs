using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Flame Strike Spell Card", menuName = "Cards/SpellFlameStrike")]
public class SpellFlameStrike : SpellCard , ISpellTypeTile
{
    public bool CastSpell(DuelInstance duel, BoardCoords pos)
    {

        StartCast(duel, pos);

        List<UnitCard> damagedCards = duel.DuelBoard.GetCardsInRow(pos.y);
        foreach(UnitCard card in damagedCards)
        {
            ActivationInfo info = new ActivationInfo(duel);
            FireEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FireEffectTemplate);
            effect.AddEffect(card, info);
            AnimationManager.Instance.UpdateCardInfoAnimation(duel,card);
        }



        FinishCast(duel);
        return true;

    }
}