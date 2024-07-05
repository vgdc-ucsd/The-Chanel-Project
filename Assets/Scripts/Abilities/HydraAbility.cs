using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abilty Description:
// When this card survives damage, it gains one attack.
[CreateAssetMenu(menuName = "Abilites/HydraAbility")]
public class HydraAbility : Ability
{
    public override ActivationCondition Condition {
        get{ return ActivationCondition.OnReceiveDamage; }
    }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        if (c.BaseDamage > 3) return; // temp nerf for balance testing
        AnimationManager.Instance.UpdateCardAttackAnimation(Info.Duel, c, 1);

        c.baseStats.baseDamage++;
        c.BaseDamage++;
        foreach(Attack atk in c.baseStats.attacks) {
            atk.damage++;
        }
        foreach(Attack atk in c.Attacks) {
            atk.damage++;
        }

        AnimationManager.Instance.AbilityActivateAnimation(Info.Duel, c);
    }
}
