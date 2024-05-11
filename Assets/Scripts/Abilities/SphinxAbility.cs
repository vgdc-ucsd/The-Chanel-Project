using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ability Description:
// This card can only die if you hit it with the exact amount of damage as health. 
[CreateAssetMenu(menuName = "Abilites/SphinxAbility")]
public class SphinxAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnReceiveDamage; } }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        if (info.OverkillDamage > 0)
        {
            c.Health = 1;
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);

    }
}
