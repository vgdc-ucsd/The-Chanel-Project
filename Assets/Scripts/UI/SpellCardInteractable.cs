


using TMPro;
using UnityEngine;

public class SpellCardInteractable : CardInteractable
{
    public SpellCard card;



    public override void SetCardInfo()
    {
        if (card == null)
        {
            Debug.Log("Could not set card info, card is uninitialzied");
            return;
        }
        CardName.text = card.Name;
        UpdateCardInfo();
    }

    public override void TryPlayCard(BoardCoords pos)
    {
        if (DuelManager.Instance.MainDuel.DuelBoard.IsOutOfBounds(pos)) return;
        if (card.spell is ISpellTypeAny anySpell) anySpell.CastSpell();
        else if (card.spell is ISpellTypeTile tileSpell) tileSpell.CastSpell(pos);

        


        // destroy the card after successful cast

        if (handInterface != null)
        {
            handInterface.cardObjects.Remove(this);
        }
        Destroy(gameObject);
    }

    public override void UpdateCardInfo()
    {
        CardCost.text = "Mana Cost: " + card.ManaCost;
    }
}