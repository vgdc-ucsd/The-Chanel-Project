using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/FrozenEffect")]
public class FrozenEffect : StatusEffect
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnEndTurn; }
    }

    public override void Activate(UnitCard c, ActivationInfo info) {
        c.frozen = true;
        c.CanAttack = false;
        c.CanMove = false;
        base.Activate(c, info);
    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        c.frozen = true;
        c.CanAttack = false;
        c.CanMove = false;
        base.AddEffect(c, info);
    }

    public override void RemoveEffect(UnitCard c, ActivationInfo info)
    {
        c.frozen = false;
        base.RemoveEffect(c, info);
    }
}