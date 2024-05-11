using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Lotus Flower Spell Card", menuName = "Cards/SpellLotusFlower")]
public class SpellLotusFlower : SpellCard , ISpellTypeAlly
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
        ability.AddEffect(card);
        
        FinishCast(duel);
        return true;
    }
}