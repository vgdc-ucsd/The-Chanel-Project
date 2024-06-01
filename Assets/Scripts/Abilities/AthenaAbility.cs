using System.Collections.Generic;
using UnityEngine;

// Abilty Description:
// When this card is placed on the board, adjacent ally cards gain 1 HP
[CreateAssetMenu(menuName = "Abilites/AthenaAbility")]
public class AthenaAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnPlay;} }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        List<UnitCard> targetCards = new List<UnitCard>();

        foreach (UnitCard cAdj in Info.Duel.DuelBoard.GetAdjacentCards(c.Pos)) {
            if (cAdj.CurrentTeam == c.CurrentTeam) {
                ++cAdj.Health;
                targetCards.Add(cAdj);
            }
        }

        if (targetCards.Count > 0) {
            AnimationManager.Instance.AbilityActivateAnimation(Info.Duel, c);
            AnimationManager.Instance.AbilityActivateAnimation(Info.Duel, targetCards);
            AnimationManager.Instance.DamageCardAnimation(Info.Duel, targetCards, Color.yellow, -1);
        }
    }
}
