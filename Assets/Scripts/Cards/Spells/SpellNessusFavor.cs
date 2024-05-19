using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Nessus's Favor Spell Card", menuName = "Cards/SpellNessusFavor")]
public class SpellNessusFavor : SpellCard , ISpellTypeAlly
{
    public NessusFavorAbility abilityTemplate;

    private void Awake()
    {
    }

    public override void CloneExtras(SpellCard copy)
    {
        if (copy is SpellNessusFavor c) c.abilityTemplate = this.abilityTemplate;
    }

    public bool CastSpell(DuelInstance duel, UnitCard card)
    {
        if (card.CurrentTeam != CurrentTeam) return false;

        NessusFavorAbility ability = ScriptableObject.Instantiate(abilityTemplate);
        ActivationInfo info = new ActivationInfo(duel);
        ability.AddEffect(card, info);
        
        FinishCast(duel);
        return true;
    }
}