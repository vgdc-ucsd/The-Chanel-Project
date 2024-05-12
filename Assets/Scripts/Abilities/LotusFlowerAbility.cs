using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/LotusFlowerAbility")]
public class LotusFlowerAbility : Ability
{
    List<Vector2Int> origDirections = new List<Vector2Int>();

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnFinishAttack; }
    }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        List<Attack> toRemove = new List<Attack>(); 
        foreach (Attack attack in c.Attacks)
        {
            if (!origDirections.Contains(attack.direction)) toRemove.Add(attack);
        }
        foreach (Attack attack in toRemove)
        {
            c.Attacks.Remove(attack);
        }

        if (c.UnitCardInteractableRef != null) c.UnitCardInteractableRef.DrawArrows();
        c.Abilities.Remove(this);
    }

    public void Init(UnitCard c)
    {
        foreach (Attack atk in c.Attacks)
        {
            origDirections.Add(atk.direction);
        }
        foreach (Vector2Int dir in Attack.allDirections)
        {
            if (c.GetAttack(dir) == null)
            {
                c.Attacks.Add(new Attack(dir, c.BaseDamage));
            }
        }
        if (c.UnitCardInteractableRef != null)
        c.UnitCardInteractableRef.DrawArrows();
    }
}
