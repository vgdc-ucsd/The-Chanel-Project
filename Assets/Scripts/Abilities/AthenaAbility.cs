using UnityEngine;

// Abilty Description:
// When this card is placed on the board, adjacent ally cards gain 1 HP
[CreateAssetMenu(menuName = "Abilites/AthenaAbility")]
public class AthenaAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnPlay;} }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        foreach (UnitCard cAdj in Info.Duel.DuelBoard.GetAdjacentCards(c.Pos)) {
            if (cAdj.CurrentTeam == c.CurrentTeam) {
                ++cAdj.Health;
                AnimationManager.Instance.DamageCardAnimation(Info.Duel, cAdj, Color.yellow);
            }
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }
}
