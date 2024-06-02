using System;
using System.Linq;
using UnityEngine;

// Abilty Description:
// When this card defeats another card, its attack is increased by 1 (does not stack)
[CreateAssetMenu(menuName = "Abilites/HerculesAbility")]
public class HerculesAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnDealDamage;} }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        if (Info.TargetCard.Health == 0 || (!Info.TargetCard.Name.Equals("Sphinx") && Info.TargetCard.Health < 0))
        {
            AnimationManager.Instance.UpdateCardAttackAnimation(Info.Duel, c, 1);

            c.baseStats.baseDamage++;
            foreach(Attack atk in c.baseStats.attacks) {
                atk.damage++;
            }
            c.RecalculateStats(Info);

            AnimationManager.Instance.AbilityActivateAnimation(Info.Duel, c);

            c.Abilities.Remove(this);
        }
    }
}
