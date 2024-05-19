


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
        else if (card is ISpellTypeUnit unitSpell)
        {
            UnitCard target = DuelManager.Instance.MainDuel.DuelBoard.GetCard(pos);
            Team targetTeam = Team.Neutral;
            if (card is ISpellTypeEnemy) targetTeam = CharStatus.OppositeTeam(card.CurrentTeam);
            else if (card is ISpellTypeAlly) targetTeam = card.CurrentTeam;
            if (target != null && targetTeam == Team.Neutral) success = unitSpell.CastSpell(DuelManager.Instance.MainDuel, target);
            else if (target != null && target.CurrentTeam == targetTeam) success = unitSpell.CastSpell(DuelManager.Instance.MainDuel, target);
        }

        if (!success) return;
        // destroy the card after successful cast

        if (handInterface != null)
        {
            handInterface.cardObjects.Remove(this.gameObject);
        }
        UIManager.Instance.UpdateStatus(DuelManager.Instance.MainDuel);
        UIManager.Instance.Player.UnhoverMana(DuelManager.Instance.MainDuel.PlayerStatus);
        inHand = false;
        /*
        UIManager.Instance.UpdateStatus(DuelManager.Instance.MainDuel);
        Destroy(gameObject); */
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if (mode != CIMode.Duel) return;
        UIManager.Instance.InfoPanel.UpdateInfoPanelSpellCard(this.card);
        if(!CanInteract || !inHand) return;
        AnimationManager.Instance.StartManaHover(card.ManaCost, card.CurrentTeam);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        if (mode != CIMode.Duel) return;
        AnimationManager.Instance.StopManaHover(card.CurrentTeam);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (mode == CIMode.Inventory)
        {
            InventoryUI.Instance.HandleClick(card);
        }
    }

    public override void UpdateCardInfo()
    {
        CardCost.text = "Mana Cost: " + card.ManaCost;
    }

    public override Card GetCard()
    {
        return card;
    }
}