using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Drowsy Spell Spell Card", menuName = "Cards/SpellFreeze")]
public class SpellFreeze : SpellCard , ISpellTypeEnemy
{

    private void Awake()
    {
    }

    public override void CloneExtras(SpellCard copy)
    {
    }

    public bool CastSpell(DuelInstance duel, UnitCard card)
    {
        if (card == null) return false;
        if (card.CurrentTeam == CurrentTeam) return false;

        StartCast(duel, card.Pos);

        FrozenEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FrozenEffectTemplate);
        ActivationInfo info = new ActivationInfo(duel);
        effect.AddEffect(card, info);
        
        FinishCast(duel);
        return true;
    }
}