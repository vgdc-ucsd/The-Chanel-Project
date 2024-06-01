using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/PoisonEffect")]
public class PoisonEffect : StatusEffect
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnEndTurn; }
    }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        info.Duel.DealDamage(c, 1);
        if (c.BaseDamage > 1) {
            c.BaseDamage--;
            foreach(Attack atk in c.Attacks) {
                atk.damage--;
            }
        }
        AnimationManager.Instance.UpdateCardAttackAnimation(info.Duel, c, -1);
        base.Activate(c, info);
    }
}
