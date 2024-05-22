using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/FireEffect")]
public class FireEffect : StatusEffect
{
    static int initialDuration = 3;

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnBeginTurn; }
    }


    public override void Activate(UnitCard c, ActivationInfo info)
    {
        if (duration != initialDuration)
        {
            info.Duel.DealDamage(c, 1);
        }
        duration--;
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
        if (duration == 0)
        {
            RemoveEffect(c, info);
        }

    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        base.AddEffect(c, info);
        duration = initialDuration;
        ReapplyEffect(c, info);
    }


    public override void ReapplyEffect(UnitCard c, ActivationInfo info)
    {
    }
}
