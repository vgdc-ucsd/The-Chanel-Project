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
        Vector2Int forward;
        //Vector2Int doubleforward;

        if (c.CurrentTeam == Team.Player)
        {
            forward = new Vector2Int(0, 1);
            //doubleforward = new Vector2Int(0, 2);
        }
        else
        {
            forward = new Vector2Int(0, -1);
            //doubleforward = new Vector2Int(0, -2);
        }
        if (Info.OverkillDamage > 0)
        {
            UnitCard middleCard = Info.Duel.DuelBoard.GetFrontCard(Info.TargetCard.Pos, c.CurrentTeam);
            Debug.Log(middleCard);
            if (middleCard == null)
            {
                return;
            }//board.getpositiontargetcard then add forward direction to it
            UnitCard backCard = Info.Duel.DuelBoard.GetFrontCard(middleCard.Pos, c.CurrentTeam);
            Debug.Log(backCard);
            /*
            BoardCoords doubleForwardPos = Info.Duel.DuelBoard.GetFrontTile(middleCard.Pos, c.CurrentTeam);
            UnitCard backCard = Info.Duel.DuelBoard.GetCard(doubleForwardPos);
            */
            if (backCard != null)
            {
                Info.Duel.DealDamage(backCard, Info.OverkillDamage);
            }

        }

        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }
}