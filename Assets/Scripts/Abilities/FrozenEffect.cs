using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/FrozenEffect")]
public class FrozenEffect : StatusEffect
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnBeginTurn; }
    }

    public override void Activate(UnitCard c, ActivationInfo info) {
        base.Activate(c, info);
        c.CanMove = false;
        c.CanAttack = false;
    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        base.AddEffect(c, info);
        c.CanMove = false;
        c.CanAttack = false;
    }

    public override void RemoveEffect(UnitCard c, ActivationInfo info)
    {
        base.RemoveEffect(c, info);
        c.CanMove = true;
        c.CanAttack = true;
    }
}