using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI Description;
    public Transform CardHolder;

    private CardInteractable currentCard;

    public Card blankCard; //temp fix for initialization

    // Leave blank if Combat
    [Header("Inventory")]
    [Space(10)]
    public TextMeshProUGUI Mana;
    public TextMeshProUGUI Health;
    public TextMeshProUGUI Atk;

    void Awake() {
        InitializeCardInfoPanel(blankCard);
    }

    public void InitializeCardInfoPanel(Card c) {
        if (c is UnitCard) {
            UpdateInfoPanelUnitCard((UnitCard)c);
        }
        else if (c is SpellCard) {
            UpdateInfoPanelSpellCard((SpellCard)c);
        }
        else {
            CardName.text = "";
            Description.text = "";
            Mana.text = "";
            Health.text = "";
            Atk.text = "";
        }
    }

    public void UpdateInfoPanelUnitCard(UnitCard uc) {
        CardName.text = uc.Name;
        Description.text = "<B>Abilities:</B>\n\n" + (uc.description.Equals("") ? "None" : uc.description);
        if(currentCard != null) Destroy(currentCard.gameObject);
        SetCardInteractable(uc);
        ((UnitCardInteractable)currentCard).DrawArrows();
    }

    public void UpdateInventoryInfoPanelUnitCard(UnitCard uc)
    {
        CardName.text = uc.Name;
        if (Description.text == "") uc.description = "Description unwritten";
        else Description.text = uc.description;

        Mana.text = uc.ManaCost + "";
        Health.text = uc.Health + "";
        Atk.text = uc.BaseDamage + "";

        // Show card
        if (currentCard != null) Destroy(currentCard.gameObject);
        SetCardInteractable(uc);
        ((UnitCardInteractable)currentCard).DrawArrows();
    }

    public void UpdateInfoPanelSpellCard(SpellCard sc) {
        CardName.text = sc.Name;
        if (Description.text == "") sc.description = "Description unwritten";
        else Description.text = sc.description;

        if (currentCard != null) Destroy(currentCard.gameObject);
        SetCardInteractable(sc);
    }

    public void SetCardInteractable(Card c) {
        // TODO use base stats
        Card card = c.Clone();
        card.CurrentTeam = Team.Neutral;
        currentCard = UIManager.Instance.GenerateCardInteractable(card);
        currentCard.transform.SetParent(CardHolder);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localScale = Vector3.one;
    }
}
