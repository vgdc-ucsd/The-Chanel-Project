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


    PersistentData.InventoryData inventory;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        inventory = PersistentData.Instance.Inventory;
        InitCards();
        ArrangeCards();
        
    }

    public void InitCards()
    {
        foreach (Card card in inventory.InactiveCards)
        {
            CardInteractable ci = card.GenerateCardInteractable();
            ci.mode = CIMode.Inventory;
            ciList.Add(ci);
         
        }
        foreach (Card card in inventory.ActiveCards)
        {
            CardInteractable ci = card.GenerateCardInteractable();
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

    }

    public void HandleClick(Card card)
    {
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
