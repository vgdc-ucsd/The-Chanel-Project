using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Lotus Flower Spell Card", menuName = "Cards/SpellLotusFlower")]
public class SpellLotusFlower : SpellCard , ISpellTypeUnit
{
    public LotusFlowerAbility abilityTemplate;

    private void Awake()
    {
    }

    public override void CloneExtras(SpellCard copy)
    {
        if (copy is SpellLotusFlower c) c.abilityTemplate = this.abilityTemplate;
    }

    public bool CastSpell(DuelInstance duel, UnitCard card)
    {
        if (card.CurrentTeam != CurrentTeam) return false;

        LotusFlowerAbility ability = ScriptableObject.Instantiate(abilityTemplate);
        card.Abilities.Add(ability);
        ability.Init(card);
        
        FinishCast(duel);
        return true;
    }
}