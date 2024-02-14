using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] Transform inventoryContainer;
    private Transform inventorySlotTemplate;

    // This method assigns inventorySlotContainer & inventorySlotTemplate
    private void Awake()
    {
        // NOTE: InventorySlotTemplate must be named exacty this.
        inventorySlotTemplate = inventoryContainer.Find("InventorySlotTemplate");
    }

    // This method reloads/refreshes the inventory UI
    public void RefreshInventoryItems()
    {
        // Delete existing cards in the UI to clear the UI
        foreach (Transform child in inventoryContainer)
        {
            // Delete every card except the template card because the template card
            // is used to create other cards
            if (child != inventorySlotTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        // Sort inventory by name (in ascending order)
        // THIS CAN BE CHANGED --> EITHER DON'T SORT AT ALL OR SORT BY RARITY FOR EXAMPLE
        if (inventoryManager.cards.Count > 0)
        {
            inventoryManager.cards = inventoryManager.SortItemsByName(inventoryManager.cards, 0, inventoryManager.cards.Count - 1);
        }

        // Add/Instantiate all cards in inventory to the UI
        foreach (Card card in inventoryManager.cards)
        {
            RectTransform inventorySlotRectTransform = Instantiate(inventorySlotTemplate, inventoryContainer).GetComponent<RectTransform>();
            inventorySlotRectTransform.gameObject.SetActive(true);

            // Set name, health, attack text appropriately
            inventorySlotRectTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = card.name;
            inventorySlotRectTransform.Find("Health").GetComponent<TextMeshProUGUI>().text = "Health: " + card.Health.ToString();
            inventorySlotRectTransform.Find("Attack").GetComponent<TextMeshProUGUI>().text = "Attack: " + card.Attack.ToString();

            // Set card info appropriately
            inventorySlotRectTransform.GetComponent<InventoryItemInteractable>().card = card;
        }
    }
}
