using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/FrozenEffect")]
public class FrozenEffect : StatusEffect
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnEndTurn; }
    }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        if (--duration < 1) {
            RemoveEffect(c, info);
        }
        c.CanMove = false;
        c.CanAttack = false;
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

    public override void RemoveEffect(UnitCard c, ActivationInfo info)
    {
        c.CanMove = true;
        c.CanAttack = true;
        base.RemoveEffect(c, info);
    }


    public override void ReapplyEffect(UnitCard c, ActivationInfo info)
    {
    }
}