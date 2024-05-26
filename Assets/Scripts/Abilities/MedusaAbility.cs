using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/MedusaAbility")]
public class MedusaAbility : Ability
{

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnPlay; }
    }

    // "Activate" is actually the finish condition, removes the extra attacks
    public override void Activate(UnitCard c, ActivationInfo info)
    {
        foreach (UnitCard card in info.Duel.DuelBoard.GetFrontCard(c.Pos))
        {
            FrozenEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FrozenEffectTemplate);
            effect.AddEffect(card, info);
        }
    }



}