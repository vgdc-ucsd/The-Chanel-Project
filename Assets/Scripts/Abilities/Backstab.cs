using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/Backstab")]
public class Backstab : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnPlay;} }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        Vector2Int backwards = new Vector2Int(0, -1);

        foreach(Attack atk in c.baseStats.attacks) {
            if(atk.direction == backwards) {
                atk.damage *= 2;
            }
        }
        c.RecalculateStats(info);
    }
}
