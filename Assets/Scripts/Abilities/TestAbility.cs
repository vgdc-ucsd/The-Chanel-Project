using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/TestAbility")]
public class TestAbility : Ability
{
    public override ActivationCondition Condition {
        get{ return ActivationCondition.OnPlay; }
    }
    
    public override void Activate(UnitCard c, ActivationInfo info) {
        //BoardCoords pos = c.GetTile().location;
        //Debug.Log(c.Name + " activated ability at (" + pos.x + ", " + pos.y + ")");
        //Debug.Log("here");
    }
}
