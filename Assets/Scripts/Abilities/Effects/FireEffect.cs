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
        base.Activate(c, info);
    }
}
