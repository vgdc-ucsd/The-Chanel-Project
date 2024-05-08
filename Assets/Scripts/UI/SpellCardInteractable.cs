


using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (!DuelManager.Instance.MainDuel.GetStatus(Team.Player).CanUseMana(card.ManaCost)) return;
        if (!DuelManager.Instance.MainDuel.GetStatus(Team.Player).CanUseMana(card.ManaCost))
        {
            Debug.Log("Not enough Mana"); //TODO: UI feedback
            return;
        }

        bool success = false;
        if (card is ISpellTypeAny anySpell) success = anySpell.CastSpell(DuelManager.Instance.MainDuel);
        else if (card is ISpellTypeTile tileSpell) success = tileSpell.CastSpell(DuelManager.Instance.MainDuel, pos);


        
        if (!success) return;

        DuelManager.Instance.MainDuel.GetStatus(Team.Player).UseMana(card.ManaCost);

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