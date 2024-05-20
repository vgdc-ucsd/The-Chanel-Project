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
        if (card == null) return false;
        if (card.CurrentTeam != CurrentTeam) return false;

        StartCast(duel, card.Pos);
        LotusFlowerAbility ability = ScriptableObject.Instantiate(abilityTemplate);
        ActivationInfo info = new ActivationInfo(duel);
        ability.AddEffect(card, info);
        FinishCast(duel);
        return true;
    }
}