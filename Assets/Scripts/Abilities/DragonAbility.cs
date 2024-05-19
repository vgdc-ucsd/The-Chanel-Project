using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/DragonAbility")]
public class DragonAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnPlay;} }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        foreach (UnitCard cCol in Info.Duel.DuelBoard.GetCardsInColumn(c.Pos.x)) {
            if (c.CurrentTeam == Team.Enemy)
            {
                if (cCol.Pos.y < c.Pos.y)
                {
                    ApplyFire(cCol, Info);
                }
            }
            else
            {
                if (cCol.Pos.y > c.Pos.y)
                {
                    ApplyFire(cCol, Info);
                }
            }
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }

    private void ApplyFire(UnitCard card, ActivationInfo info)
    {
        FireEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FireEffectTemplate);
        effect.AddEffect(card, info);
    }
}
