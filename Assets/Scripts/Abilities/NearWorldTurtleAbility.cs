using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/NearWorldTurtleAbility")]
public class NearWorldTurtleAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnProcess; } } // After the end turn

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        foreach (UnitCard adjCard in info.Duel.DuelBoard.GetAdjacentCards(c.Pos))
        {
            if (adjCard.Name == "WorldTurtle") // edit
            {
                // redirect damage to 
                adjCard.TakeDamage(info.Duel, 0); // 0 redirected damage;
                //adjCard.UnitCardInteractableRef.gameObject.GetComponent<WorldTurtleAbility>().damageAbsorbed += dmg;
            }
        }

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
