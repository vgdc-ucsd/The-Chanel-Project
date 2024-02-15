using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public int maxItems = 10;
    public List<InventoryItemData> items;
    public InventoryUI inventoryUI;

    void Start()
    {
        // Create a new list at the start
        InitializeInventory();

        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    private void InitializeInventory()
    {
        items = new List<InventoryItemData>(maxItems);
    }

    public void AddItem(InventoryItemData inventoryItem)
    {
        if (items.Count < maxItems)
        {
            items.Add(inventoryItem);
        }
        else
        {
            Debug.Log("No space in inventory");
        }

        inventoryUI.RefreshInventoryItems();
    }

    public void RemoveItem(InventoryItemData itemToRemove)
    {
        items.Remove(itemToRemove);

        inventoryUI.RefreshInventoryItems();
    }

    // Quicksort to sort 'items' list by name (in ascending order)
    public List<InventoryItemData> SortItemsByName(List<InventoryItemData> inventoryItemDatas, int leftIndex, int rightIndex)
    {
        var i = leftIndex;
        var j = rightIndex;
        var pivot = inventoryItemDatas[leftIndex];

        while (i <= j)
        {
            while (String.Compare(inventoryItemDatas[i].name, pivot.name) < 0)
            // inventoryItemDatas[i].name < pivot.name)
            {
                i++;
            }

            while (String.Compare(inventoryItemDatas[j].name, pivot.name) > 0)
            {
                j--;
            }

            if (i <= j)
            {
                InventoryItemData temp = inventoryItemDatas[i];
                inventoryItemDatas[i] = inventoryItemDatas[j];
                inventoryItemDatas[j] = temp;
                i++;
                j--;
            }
        }

        if (leftIndex < j)
            SortItemsByName(inventoryItemDatas, leftIndex, j);

        if (i < rightIndex)
            SortItemsByName(inventoryItemDatas, i, rightIndex);

        return inventoryItemDatas;
    }
}
