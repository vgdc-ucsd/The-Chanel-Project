using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] ShopManager shopManager;
    private Transform shopSlotContainer;
    private Transform shopSlotTemplate;

    // This method assigns shopSlotContainer & shopSlotTemplate
    private void Awake()
    {
        shopSlotContainer = transform.Find("Scroll View/Viewport/ShopSlotContainer");
        shopSlotTemplate = shopSlotContainer.Find("ShopSlotTemplate");
    }

    // This method reloads/refreshes the shop UI
    public void RefreshShopItems()
    {
        // Delete existing cards in the UI
        foreach (Transform child in shopSlotContainer)
        {
            // Delete every card except the template card because the template card
            // is used to create other cards
            if (child != shopSlotTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        // Sort shop by name (in ascending order)
        // THIS CAN BE CHANGED --> EITHER DON'T SORT AT ALL OR SORT BY RARITY FOR EXAMPLE
        if (shopManager.items.Count > 0)
        {
            shopManager.items = shopManager.SortItemsByName(shopManager.items, 0, shopManager.items.Count - 1);
        }

        // Add all cards in shop to the UI
        foreach (ShopItemData item in shopManager.items)
        {
            RectTransform shopSlotRectTransform = Instantiate(shopSlotTemplate, shopSlotContainer).GetComponent<RectTransform>();
            shopSlotRectTransform.gameObject.SetActive(true);

            // Set name, health, attack text appropriately
            shopSlotRectTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
            shopSlotRectTransform.Find("Health").GetComponent<TextMeshProUGUI>().text = "Health: " + item.health.ToString();
            shopSlotRectTransform.Find("Attack").GetComponent<TextMeshProUGUI>().text = "Attack: " + item.attack.ToString();
            shopSlotRectTransform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Cost: " + item.cost.ToString();

            // Set card info appropriately
            shopSlotRectTransform.GetComponent<ShopItemInteractable>().item = item;
        }
    }
}

