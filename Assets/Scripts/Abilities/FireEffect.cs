using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/FireEffect")]
public class FireEffect : StatusEffect
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnBeginTurn; }
    }


    public override void Activate(UnitCard c, ActivationInfo info)
    {
        info.Duel.DealDamage(c, 1);
        if (--duration < 1) {
            RemoveEffect(c, info);
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
