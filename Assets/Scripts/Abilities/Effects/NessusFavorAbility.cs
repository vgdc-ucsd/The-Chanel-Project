using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/NessusFavorAbility")]
public class NessusFavorAbility : StatusEffect
{

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnDealDamage; }
    }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        if (info.TargetCard.Health > 0) {
            PoisonEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.PoisonEffectTemplate);
            effect.AddEffect(info.TargetCard, info);
        }
    }
}
