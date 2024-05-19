using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/CerberusAbility")]
public class CerberusAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnAttacksHitMe; } }

    public FireEffect effectTemplate; // probably better to have a universal reference to all effects somewhere
    public override void Activate(UnitCard c, ActivationInfo info)
    {
        ApplyFire(c, info);
        info.Duel.DealDamage(c, 1);

        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }

    private void ApplyFire(UnitCard card, ActivationInfo info)
    {
        FireEffect effect = ScriptableObject.Instantiate(effectTemplate);
        effect.AddEffect(card, info);
    }
}
