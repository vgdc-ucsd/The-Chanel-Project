using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
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
        RemoveEffect(c);
        if (c.UnitCardInteractableRef != null) c.UnitCardInteractableRef.DrawArrows();
    }

    public override void AddEffect(UnitCard c)
    {
        base.AddEffect(c);

        ReapplyEffect(c);
    }

    public override void ReapplyEffect(UnitCard c)
    {
        foreach (Vector2Int dir in Attack.allDirections)
        {
            if (c.GetAttack(dir) == null)
            {
                c.Attacks.Add(new Attack(dir, c.BaseDamage));
            }
        }
        if (c.UnitCardInteractableRef != null) c.UnitCardInteractableRef.DrawArrows();
    }
}
