using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/PoisonEffect")]
public class PoisonEffect : StatusEffect
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnBeginTurn; }
    }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        info.Duel.DealDamage(c, 1);
        if (c.baseStats.baseDamage > 1) {
            c.baseStats.baseDamage--;
            foreach(Attack atk in c.baseStats.attacks) {
                atk.damage--;
            }
        }
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
