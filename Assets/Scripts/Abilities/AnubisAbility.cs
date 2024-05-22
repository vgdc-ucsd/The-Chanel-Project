using UnityEngine;

// Abilty Description:
// When this card dies, it sacrifices the lowest value card on the board to resurrect
[CreateAssetMenu(menuName = "Abilites/AnubisAbility")]
public class AnubisAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnDeath;} }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        UnitCard anubisCard = c;
        foreach (UnitCard card in Info.Duel.GetStatus(c.CurrentTeam).Deck.CardList) {
            if (card.Name == c.Name) {
                anubisCard = card;

                anubisCard.Abilities.Clear();
                anubisCard.ManaCost = 0;

                break;
            }
        }

        UnitCard lowestCard = c;
        foreach (UnitCard card in Info.Duel.DuelBoard.CardSlots) {
            if (card != null && card.ManaCost < lowestCard.ManaCost) {
                lowestCard = card;
            }
        }

        if (!lowestCard.Equals(c)) {
            Info.Duel.DealDamage(lowestCard, lowestCard.Health);
            AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, lowestCard);

            Info.Duel.GetStatus(c.CurrentTeam).Deck.CardList.Add(anubisCard);
        }

        AnimationManager.Instance.UpdateCardInfoAnimation(Info.Duel, c);
    }
}
