using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/LotusFlowerAbility")]
public class LotusFlowerAbility : StatusEffect
{
    

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnFinishAttack; }
    }


    // "Activate" is actually the finish condition, removes the extra attacks
    public override void Activate(UnitCard c, ActivationInfo info)
    {
        RemoveEffect(c, info);
        AnimationManager.Instance.DrawArrowsAnimation(info.Duel, c);
    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        base.AddEffect(c, info);
        duration = -1;
        ReapplyEffect(c, info);
    }


    public override void ReapplyEffect(UnitCard c, ActivationInfo info)
    {
        foreach (Vector2Int dir in Attack.allDirections)
        {
            if (c.GetAttack(dir) == null)
            {
                c.Attacks.Add(new Attack(dir, c.BaseDamage));
            }
        }
        AnimationManager.Instance.DrawArrowsAnimation(info.Duel, c);
    }
}
