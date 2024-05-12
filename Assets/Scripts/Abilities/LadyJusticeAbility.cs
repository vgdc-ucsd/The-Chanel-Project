using UnityEngine;

// Abilty Description:
// When this card attacks another card, that card will instantly die if its resulting health is not even
[CreateAssetMenu(menuName = "Abilites/LadyJusticeAbility")]
public class LadyJusticeAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnDealDamage;} }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        if (Info.TargetCard.Health % 2 != 0) {
            Info.Duel.DealDamage(Info.TargetCard, Info.TargetCard.Health);
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }
}
