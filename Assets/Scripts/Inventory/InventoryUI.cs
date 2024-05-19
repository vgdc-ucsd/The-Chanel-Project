using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] Transform inventoryContainer;
    [SerializeField] GameObject inventoryTemplateCardPrefab;

    List<CardInteractable> ciList = new List<CardInteractable>();

    // This method assigns inventorySlotContainer & inventoryTemplateCardPrefab
    // private void Awake()
    // {
    //     // NOTE: inventoryTemplateCardPrefab must be named exacty this.
    //     inventoryTemplateCardPrefab = inventoryContainer.Find("inventoryTemplateCardPrefab");
    // }

    private void Awake()
    {
        InitCards();
        ArrangeCards();
    }

    public void InitCards()
    {
        foreach (Card card in PersistentData.Instance.Inventory.InactiveCards)
        {
            CardInteractable ci = card.GenerateCardInteractable();
            ci.mode = CIMode.Inventory;
            ciList.Add(ci);
            
        }
    }

    public void ArrangeCards()
    {

        for (int i = 0; i < ciList.Count; i++)
        {
            CardInteractable ci = ciList[i];
            ci.transform.SetParent(inventoryContainer.transform.GetChild(i));
            ci.transform.localScale = Vector3.one * 2;
            ci.transform.localPosition = Vector3.zero;
        }

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
