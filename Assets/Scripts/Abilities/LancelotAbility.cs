using System;
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
            BoardCoords backTile = Info.Duel.DuelBoard.GetFrontTile(Info.TargetCard.Pos, c.CurrentTeam);
            if (Info.Duel.DuelBoard.BeyondEnemyEdge(backTile) && c.CurrentTeam == Team.Player)
            {
                Info.Duel.EnemyStatus.TakeDamage(Info.OverkillDamage);
            }
            else if (Info.Duel.DuelBoard.BeyondPlayerEdge(backTile) && c.CurrentTeam == Team.Enemy)
            {
                Info.Duel.PlayerStatus.TakeDamage(Info.OverkillDamage);
            }
            else
            {
                UnitCard backCard = Info.Duel.DuelBoard.GetFrontCard(Info.TargetCard.Pos, c.CurrentTeam);
                if (backCard != null)
                {
                    Info.Duel.DealDamage(backCard, Info.OverkillDamage);
                }
            }
            AnimationManager.Instance.AbilityActivateAnimation(Info.Duel, c);
            AnimationManager.Instance.UpdateUIAnimation(Info.Duel);
            AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
        }

    }
}