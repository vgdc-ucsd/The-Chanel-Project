using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/WorldTurtleAbility")]
public class WorldTurtleAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnReceiveDamage; } } // prob not OnRecieveDmg

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        // Gather all the adjacent ally cards
        // OnTakeDamage -> Gather all the damages

        int totalDamageTaken = 0;
        // maybe make an array of all the card adjacent cards? UnitCard[] adjacentCards = {};

        foreach (UnitCard card in info.Duel.DuelBoard.GetAdjacentCards(c.Pos))
        {
            // totalDamageTaken = card.damageThatWasTaken;
            // issue rn is waiting for all the cards to recieve damage first, then taking it 
            totalDamageTaken += card.LastDamageTaken;
        }

        c.TakeDamage(info.Duel, totalDamageTaken);
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }
}
