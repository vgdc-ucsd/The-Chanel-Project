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
        if (--duration < 1) {
            RemoveEffect(c, info);
        }
        if (info.TargetCard.Health > 0) {
            PoisonEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.PoisonEffectTemplate);
            effect.AddEffect(info.TargetCard, info);
        }
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        StatusEffect cardStatus = c.GetStatusEffect(this);
        if (cardStatus != null) {
            cardStatus.duration += initialDuration;
        }
        else {
            duration = initialDuration;
            c.Abilities.Add(this);
            c.StatusEffects.Add(this);
        }
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }

    public override void ReapplyEffect(UnitCard c, ActivationInfo info)
    {
    }
}
