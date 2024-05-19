using UnityEngine;

// Abilty Description:
// When this card is killed, the burn effect is applied to adjacent enemies
[CreateAssetMenu(menuName = "Abilites/PhoenixAbility")]
public class PhoenixAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnDeath;} }

    public FireEffect effectTemplate;

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        foreach (UnitCard cAdj in Info.Duel.DuelBoard.GetAdjacentCards(c.Pos)) {
            if (cAdj.CurrentTeam != c.CurrentTeam) {
                FireEffect effect = ScriptableObject.Instantiate(effectTemplate);
                effect.AddEffect(cAdj, Info);

                AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, cAdj);
            }
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }
}
