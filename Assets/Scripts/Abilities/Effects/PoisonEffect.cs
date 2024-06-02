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
        base.Activate(c, info);
        if (c.BaseDamage > 1) {
            AnimationManager.Instance.UpdateCardAttackAnimation(info.Duel, c, -1);
            c.baseStats.baseDamage--;
            c.BaseDamage--;
            foreach(Attack atk in c.baseStats.attacks) {
                atk.damage--;
            }
            foreach(Attack atk in c.Attacks) {
                atk.damage--;
            }
        }
        info.Duel.DealDamage(c, 1);
    }
}
