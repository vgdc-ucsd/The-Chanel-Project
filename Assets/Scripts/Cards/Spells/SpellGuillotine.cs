using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Guillotine Spell Card", menuName = "Cards/SpellGuillotine")]
public class SpellGuilltine : SpellCard , ISpellTypeEnemy
{
    public bool CastSpell(DuelInstance duel, UnitCard card)
    {
        if (card == null) return false;
        if (card.CurrentTeam == CurrentTeam) return false;

        StartCast(duel, card.Pos);

        duel.DealDamage(card, card.ManaCost, true);
        AnimationManager.Instance.UpdateCardInfoAnimation(duel, card);

        FinishCast(duel);
        return true;

    }
}