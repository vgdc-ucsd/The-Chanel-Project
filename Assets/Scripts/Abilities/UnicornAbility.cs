using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Ability Description:
// If this card attacks forward, it deals double damage
[CreateAssetMenu(menuName = "Abilites/UnicornAbility")]
public class UnicornAbility : Ability
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnPlay; }
    }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        Vector2Int forward;
        if (c.CurrentTeam == Team.Player)
        {
            forward = new Vector2Int(0, 1);
        }
        else
        {
            forward = new Vector2Int(0, -1);
        }
        foreach (Attack attackdir in c.Attacks)
        {
            if (attackdir.direction == forward)
            {
                attackdir.damage *= 2;
            }
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }
}