using UnityEngine;

// Abilty Description:
// When this card is killed, the burn effect is applied to adjacent enemies
[CreateAssetMenu(menuName = "Abilites/PhoenixAbility")]
public class PhoenixAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnDeath;} }


    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        foreach (UnitCard cAdj in Info.Duel.DuelBoard.GetAdjacentCards(c.Pos)) {
            if (cAdj.CurrentTeam != c.CurrentTeam) {
                AnimationManager.Instance.AbilityActivateAnimation(Info.Duel, c);
                FireEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FireEffectTemplate);
                effect.initialDuration = 3;
                effect.AddEffect(cAdj, Info);

                AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, cAdj);
            }
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }
}
