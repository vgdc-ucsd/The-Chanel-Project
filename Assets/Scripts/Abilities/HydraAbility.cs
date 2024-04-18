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

    public override void Activate(Board b, UnitCard c, bool mainDuel)
    {
        // Add 1 damage to each attack
        c.BaseDamage++;
        foreach(Attack atk in c.Attacks) {
            atk.damage++;
        }

        // Update UI
        if(mainDuel) {
            c.CardInteractableRef.UpdateCardInfo();
        }
    }
}
