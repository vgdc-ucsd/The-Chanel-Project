using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int maxItems = 10;
    public List<ShopItemData> items;
    public ShopUI shopUI;

    void Start()
    {
        // Create a new list at the start
        InitializeShop();

        shopUI = FindObjectOfType<ShopUI>();
    }

    private void InitializeShop()
    {
        items = new List<ShopItemData>(maxItems);
    }

    public void AddItem(ShopItemData shopItem)
    {
        if (items.Count < maxItems)
        {
            items.Add(shopItem);
        }
        else
        {
            Debug.Log("No space in shop");
        }

        shopUI.RefreshShopItems();
    }

    public void RemoveItem(ShopItemData itemToRemove)
    {
        items.Remove(itemToRemove);

        shopUI.RefreshShopItems();
    }

    // Quicksort to sort 'items' list by name (in ascending order)
    public List<ShopItemData> SortItemsByName(List<ShopItemData> shopItemDatas, int leftIndex, int rightIndex)
    {
        var i = leftIndex;
        var j = rightIndex;
        var pivot = shopItemDatas[leftIndex];

        while (i <= j)
        {
            while (String.Compare(shopItemDatas[i].name, pivot.name) < 0)
            // shopItemDatas[i].name < pivot.name)
            {
                i++;
            }

            while (String.Compare(shopItemDatas[j].name, pivot.name) > 0)
            {
                j--;
            }

            if (i <= j)
            {
                ShopItemData temp = shopItemDatas[i];
                shopItemDatas[i] = shopItemDatas[j];
                shopItemDatas[j] = temp;
                i++;
                j--;
            }
        }

        if (leftIndex < j)
            SortItemsByName(shopItemDatas, leftIndex, j);

        if (i < rightIndex)
            SortItemsByName(shopItemDatas, i, rightIndex);

        return shopItemDatas;
    }
}
