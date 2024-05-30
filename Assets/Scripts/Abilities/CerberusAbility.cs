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
        if (c.Health > 0) {
            ApplyFire(c, info);
        }
    }

    private void ApplyFire(UnitCard card, ActivationInfo info)
    {
        FireEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FireEffectTemplate);
        effect.initialDuration = 3;
        effect.AddEffect(card, info);
    }
}
