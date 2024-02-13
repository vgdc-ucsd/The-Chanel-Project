using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    private Transform inventorySlotContainer;
    private Transform inventorySlotTemplate;

    // This method assigns inventorySlotContainer & inventorySlotTemplate
    private void Awake()
    {
        inventorySlotContainer = transform.Find("InventorySlotContainer");
        inventorySlotTemplate = inventorySlotContainer.Find("InventorySlotTemplate");
    }

    // This method reloads/refreshes the inventory UI
    public void RefreshInventoryItems()
    {
        // Delete existing cards in the UI
        foreach (Transform child in inventorySlotContainer)
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
        if (inventoryManager.items.Count > 0)
        {
            inventoryManager.items = inventoryManager.SortItemsByName(inventoryManager.items, 0, inventoryManager.items.Count - 1);
        }

        // Add all cards in inventory to the UI
        foreach (InventoryItemData item in inventoryManager.items)
        {
            RectTransform inventorySlotRectTransform = Instantiate(inventorySlotTemplate, inventorySlotContainer).GetComponent<RectTransform>();
            inventorySlotRectTransform.gameObject.SetActive(true);

            // Set name, health, attack text appropriately
            inventorySlotRectTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
            inventorySlotRectTransform.Find("Health").GetComponent<TextMeshProUGUI>().text = "Health: " + item.health.ToString();
            inventorySlotRectTransform.Find("Attack").GetComponent<TextMeshProUGUI>().text = "Aattack: " + item.attack.ToString();

            // Set card info appropriately
            inventorySlotRectTransform.GetComponent<InventoryItemInteractable>().item = item;
        }
    }
}
