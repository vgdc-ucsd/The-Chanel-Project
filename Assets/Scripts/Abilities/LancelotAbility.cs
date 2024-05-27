using UnityEngine;

// Abilty Description:
// When this card attacks another card, overkill damage is directly applied to the opponent's health
[CreateAssetMenu(menuName = "Abilites/LancelotAbility")]
public class LancelotAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnDealDamage; } }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        if (Info.OverkillDamage > 0)
        {
            /*
            if(UnitCard card = )
            */
        }
        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }
}