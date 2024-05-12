using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Red Spider Lily Spell Card", menuName = "Cards/SpellRedSpiderLily")]
public class SpellRedSpiderLily : SpellCard , ISpellTypeAlly
{
    public RedSpiderLilyAbility abilityTemplate;

    private void Awake()
    {
    }

    public override void CloneExtras(SpellCard copy)
    {
        if (copy is SpellRedSpiderLily c) c.abilityTemplate = this.abilityTemplate;
    }

    public bool CastSpell(DuelInstance duel, UnitCard card)
    {
        if (card.CurrentTeam != CurrentTeam) return false;

        RedSpiderLilyAbility ability = ScriptableObject.Instantiate(abilityTemplate);
        ability.AddEffect(card);
        
        FinishCast(duel);
        return true;
    }
}