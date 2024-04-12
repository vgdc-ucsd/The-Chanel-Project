using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/TestAbility")]
public class TestAbility : Ability
{
    public override ActivationCondition Condition {
        get{ return ActivationCondition.OnProcess; }
    }
    
    public override void Activate(Board b, Card c) {
        BoardCoords pos = c.GetTile().location;
        //Debug.Log(c.Name + " activated ability at (" + pos.x + ", " + pos.y + ")");
    }
}
