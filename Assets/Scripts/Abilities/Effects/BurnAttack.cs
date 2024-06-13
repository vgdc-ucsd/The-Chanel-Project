using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/BurnAttack")]
public class BurnAttack : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnAttack; } } // Can change to OnDealDamage, just wanted to safely ensure burn isn't applied to dead card

    public FireEffect effectTemplate; // probably better to have a universal reference to all effects somewhere
    public override void Activate(UnitCard c, ActivationInfo info)
    {
        ApplyFire(info.TargetCard, info);
    }

    private void ApplyFire(UnitCard card, ActivationInfo info)
    {
        FireEffect effect = ScriptableObject.Instantiate(effectTemplate);
        effect.AddEffect(card, info);
    }
}
