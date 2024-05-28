using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/CerberusAbility")]
public class CerberusAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnAttacksHitMe; } }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        AnimationManager.Instance.AbilityActivateAnimation(info.Duel, c);
        info.Duel.DealDamage(c, 1);
        ApplyFire(c, info);
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }

    private void ApplyFire(UnitCard card, ActivationInfo info)
    {
        FireEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FireEffectTemplate);
        effect.AddEffect(card, info);
    }
}
