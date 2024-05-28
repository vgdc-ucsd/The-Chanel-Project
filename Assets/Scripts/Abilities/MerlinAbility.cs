using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/MerlinAbility")]
public class MerlinAbility : Ability
{
    public override ActivationCondition Condition { get { return ActivationCondition.OnDealDamage; } }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        AnimationManager.Instance.AbilityActivateAnimation(info.Duel, c);

        Board board = info.Duel.DuelBoard;

        UnitCard target = info.TargetCard;
        BoardCoords oldPos = c.Pos;
        bool swap = board.IsOccupied(target.Pos); //check if card died
        board.TeleportCard(c, target.Pos, info.Duel);
        if (swap) 
            board.TeleportCard(target, oldPos, info.Duel, false);
    }
}
