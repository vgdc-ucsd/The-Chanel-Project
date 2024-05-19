using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/PoisonEffect")]
public class PoisonEffect : StatusEffect
{
    static int initialDuration = 1;

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnEndTurn; }
    }


    public override void Activate(UnitCard c, ActivationInfo info)
    {

        info.Duel.DealDamage(c, 1);
        duration--;
        if (duration == 0)
        {
            RemoveEffect(c, info);
        }
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);

    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        base.AddEffect(c, info);
        duration = initialDuration;
        ReapplyEffect(c, info);
    }


    public override void ReapplyEffect(UnitCard c, ActivationInfo info)
    {
        foreach (Attack atk in c.Attacks)
        {
            if (atk.damage > 0) 
            { 
                atk.damage--;
            }
        }
    }
}
