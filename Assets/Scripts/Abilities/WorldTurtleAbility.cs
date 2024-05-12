using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/WorldTurtleAbility")]
public class WorldTurtleAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnReceiveDamage; } } // On after everyone's taken damage

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        int damageAbsorbed = 0;

        foreach (UnitCard adjCard in info.Duel.DuelBoard.GetAdjacentCards(c.Pos))
        {
            // Take all their dmg and do it to me
            // damageAbsorbed += adjCard's info.totalDamage
            

        }

        c.TakeDamage(info.Duel, damageAbsorbed);
    }
}

// Sequence
// All card's take damage first, so like this is a on SUPER LAST TURN (could be an on trigger)
// Get the adjacent card's totalDamage info
// Add it to damageAbsorbed
// Heal  the cards that respective damage
// Then deal it to yourself
// should it just super tank everything ?or should it go by NWSE

// ask ethan if u can get access to the dmg a card's taken
