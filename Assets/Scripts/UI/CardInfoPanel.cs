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

    void Awake()
    {
        if (InventoryUI.Instance != null) { }
            InitializeCardInfoPanel(null);
    }

    public void InitializeCardInfoPanel(Card c)
    {
        if (c is UnitCard)
        {
            UpdateInfoPanelUnitCard((UnitCard)c);
        }
        else if (c is SpellCard)
        {
            UpdateInfoPanelSpellCard((SpellCard)c);
        }
        else
        {
            CardName.text = "";
            Description.text = "";
            if (Mana != null) Mana.text = "";
            if (Health != null) Health.text = "";
            if (Atk != null) Atk.text = "";
        }
    }

    public void UpdateInfoPanelUnitCard(UnitCard uc)
    {
        EnablePanel();
        CardName.text = uc.Name;
        Description.text = "<B>Description:</B>\n" + (uc.description.Equals("") ? "None" : uc.description);
        if (Mana != null) Mana.text = uc.ManaCost + "";
        if (currentCard != null) Destroy(currentCard.gameObject);
        SetCardInteractable(uc);
        ((UnitCardInteractable)currentCard).DrawArrows();
    }

    public void UpdateInventoryInfoPanelUnitCard(UnitCard uc)
    {
        EnablePanel();
        CardName.text = uc.Name;
        Description.text = "<B>Description:</B>\n" + (uc.description.Equals("") ? "None" : uc.description);
        Mana.text = uc.ManaCost + "";
        Health.text = uc.Health + "";
        Atk.text = uc.BaseDamage + "";

        // Show card
        if (currentCard != null) Destroy(currentCard.gameObject);
        SetCardInteractable(uc);
        ((UnitCardInteractable)currentCard).DrawArrows();
        currentCard.CanInteract = false;
        currentCard.mode = CIMode.Display;
    }

    public void UpdateInfoPanelSpellCard(SpellCard sc)
    {
        EnablePanel();
        CardName.text = sc.Name;
        Description.text = "<B>Description:</B>\n" + (sc.description.Equals("") ? "None" : sc.description);
        if (Mana != null) Mana.text = sc.ManaCost + "";

        // Shows spell card
        if (currentCard != null) Destroy(currentCard.gameObject);
        SetCardInteractable(sc);

        currentCard.CanInteract = false;
    }

    public void SetCardInteractable(Card c)
    {
        // TODO use base stats
        Card card = c.Clone();
        card.CurrentTeam = Team.Neutral;
        if (card.CardInteractableRef.mode == CIMode.Inventory)
        {
            currentCard = InventoryUI.Instance.GenerateCardInteractable(card);
        }
        else
        {
            currentCard = UIManager.Instance.GenerateCardInteractable(card);
        }
        currentCard.mode = CIMode.Display;

        currentCard.GetComponent<GraphicRaycaster>().enabled = false;

        currentCard.transform.SetParent(CardHolder);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localScale = Vector3.one;
        currentCard.CardCost.enabled = true;
        currentCard.Mana.gameObject.SetActive(false);

        if (UIManager.Instance == null && InventoryUI.Instance != null)
        {
            if (c is UnitCard)
            {
                currentCard.image.sprite = c.CurrentTeam == Team.Enemy ? InventoryUI.Instance.EnemyUnitCardBorder : InventoryUI.Instance.PlayerUnitCardBorder;
            }
            else
            {
                currentCard.image.sprite = c.CurrentTeam == Team.Enemy ? InventoryUI.Instance.EnemySpellCardBorder : InventoryUI.Instance.PlayerSpellCardBorder;
            }
        }
        else
        {
            if (c is UnitCard)
            {
                currentCard.image.sprite = c.CurrentTeam == Team.Enemy ? UIManager.Instance.EnemyUnitCardBorder : UIManager.Instance.PlayerUnitCardBorder;
            }
            else
            {
                currentCard.image.sprite = c.CurrentTeam == Team.Enemy ? UIManager.Instance.EnemySpellCardBorder : UIManager.Instance.PlayerSpellCardBorder;
            }
        }
    }

    public void EnablePanel()
    {
        CardName.gameObject.SetActive(true);
        Description.gameObject.SetActive(true);
    }
}
