using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Playables; Build error
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/FrozenEffect")]
public class FrozenEffect : StatusEffect
{
    static int initialDuration = 0;

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnBeginOppositeTurn; }
    }


    public override void Activate(UnitCard c, ActivationInfo info)  
    {
        if (duration == 0)
        {
            RemoveEffect(c, info);
        }
        c.CanMove = false;
        c.CanAttack = false;
        duration--;
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
    }
}