using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    const int ROW_SIZE = 6;


    public static InventoryUI Instance;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] Transform inventoryContainer;
    public int invContainerSize = ROW_SIZE * 2;
    [SerializeField] Transform deckContainer;
    [SerializeField] GameObject inventoryTemplateCardPrefab;

    List<CardInteractable> ciList = new List<CardInteractable>();

    // Display Card
    public CardInfoPanel inventoryInfoPanel;
    public TextMeshProUGUI cardCountText;

    public TMP_Text goldCountText;

    PersistentData.InventoryData inventory;

    Coroutine deckSizeWarnCor;
    public TMP_Text deckSizeWarningText;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        inventory = PersistentData.Instance.Inventory;
        InitCards();
        ArrangeCards();

        goldCountText.text = PersistentData.Instance.Inventory.Gold.ToString();

    }

    public void InitCards()
    {
        foreach (Card card in inventory.InactiveCards)
        {
            CardInteractable ci = UIManager.Instance.GenerateCardInteractable(card);
            ci.mode = CIMode.Inventory;
            ciList.Add(ci);
         
        }
        foreach (Card card in inventory.ActiveCards)
        {
            CardInteractable ci = UIManager.Instance.GenerateCardInteractable(card);
            ci.mode = CIMode.Inventory;
            ciList.Add(ci);
        }
    }

    public void ArrangeCards()
    {
        int i = 0;
        int invIndex = 0, deckIndex = 0;
        while (i < inventory.CardCount()) 
        {
            CardInteractable ci = ciList[i];
            Card c = ci.GetCard();

            if (inventory.IsActive(c))
            {
                ci.transform.SetParent(deckContainer.transform.GetChild(deckIndex));
                deckIndex++;
            }
            else
            {
                if (invIndex >= invContainerSize)
                {
                    for (int j = 0; j < ROW_SIZE; j++)
                    {
                        GameObject slot = new GameObject();
                        slot.AddComponent<RectTransform>();
                        slot.transform.SetParent(inventoryContainer.transform);
                    }
                    invContainerSize += ROW_SIZE;
                }

                ci.transform.SetParent(inventoryContainer.transform.GetChild(invIndex));
                invIndex++;
            }
            
            
            ci.transform.localScale = Vector3.one * 2;
            ci.transform.localPosition = Vector3.zero;
            i++;
        }


        // Adds Cards Count
        cardCountText.text = inventory.CardCount() + "";
    }

    public void HandleClick(CardInteractable ci)
    {
        Card card = ci.GetCard();
        ciList.Remove(ci);
        ciList.Add(ci);
        if (inventory.IsActive(card))
        {
            Unequip(card);
        }
        else 
        {
            if (inventory.ActiveCards.Count < GameData.DECK_SIZE)
                Equip(card);
        }
    }

    private void Equip(Card card)
    {
        inventory.InactiveCards.Remove(card);
        inventory.ActiveCards.Add(card);
        ArrangeCards();
    }

    private void Unequip(Card card)
    {
        inventory.InactiveCards.Add(card);
        inventory.ActiveCards.Remove(card);
        ArrangeCards();
    }

    public void TryExit()
    {
        if (inventory.ActiveCards.Count != Deck.DECK_SIZE)
        {
            if (deckSizeWarnCor != null) StopCoroutine(deckSizeWarnCor);
            deckSizeWarnCor = StartCoroutine(DeckSizeWarn());
            return;
        }

        MenuScript.Instance.LoadPrevFromInventory();
    }

    IEnumerator DeckSizeWarn()
    {
        deckSizeWarningText.enabled = true;
        yield return new WaitForSeconds(1);
        deckSizeWarningText.enabled = false;
    }

    // This method reloads/refreshes the inventory UI
    public void RefreshInventoryItems()
    {
        // Delete existing cards in the UI to clear the UI
        foreach (Transform child in inventoryContainer)
        {
            // Delete every card except the template card because the template card
            // is used to create other cards
            if (child != inventoryTemplateCardPrefab)
            {
                Destroy(child.gameObject);
            }
        }

        // Sort inventory by name (in ascending order)
        // THIS CAN BE CHANGED --> EITHER DON'T SORT AT ALL OR SORT BY RARITY FOR EXAMPLE
        //if (inventoryManager.cards.Count > 0)
        //{
        //    inventoryManager.cards = inventoryManager.SortItemsByName(inventoryManager.cards, 0, inventoryManager.cards.Count - 1);
        //}

        // Add Instantiate all cards in inventory to the UI
        
        foreach (UnitCard card in PersistentData.Instance.Inventory.InactiveCards) 
        {

        }

    }
}
