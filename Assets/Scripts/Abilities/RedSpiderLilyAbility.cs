using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/RedSpiderLilyAbility")]
public class RedSpiderLilyAbility : StatusEffect
{

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnFinishAttack; }
    }

    // "Activate" is actually the finish condition, removes the extra attacks
    public override void Activate(UnitCard c, ActivationInfo info)
    {
        RemoveEffect(c);
    }

    public override void AddEffect(UnitCard c)
    {
        base.AddEffect(c);

        ReapplyEffect(c);
    }

    public override void ReapplyEffect(UnitCard c)
    {
        foreach (Attack atk in c.Attacks)
        {
            atk.damage *= 2;
        }
    }
}
