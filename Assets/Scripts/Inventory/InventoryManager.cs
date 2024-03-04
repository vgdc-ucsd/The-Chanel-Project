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
<<<<<<< Updated upstream
    public List<InventoryItemData> items;
=======
    // IDK IF THIS IS SUPPOSED TO BE A LIST OF CARDS OR A DECK
    public List<UnitCard> cards;
>>>>>>> Stashed changes
    public InventoryUI inventoryUI;

    void Start()
    {
        // Create a new list at the start
        InitializeInventory();

        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    private void InitializeInventory()
    {
<<<<<<< Updated upstream
        items = new List<InventoryItemData>(maxItems);
    }

    public void AddItem(InventoryItemData inventoryItem)
=======
        cards = new List<UnitCard>(maxItems);
    }

    public void AddItem(UnitCard card)
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    public void RemoveItem(InventoryItemData itemToRemove)
=======
    public void RemoveItem(UnitCard card)
>>>>>>> Stashed changes
    {
        items.Remove(itemToRemove);

        inventoryUI.RefreshInventoryItems();
    }

    // Quicksort to sort 'items' list by name (in ascending order)
<<<<<<< Updated upstream
    public List<InventoryItemData> SortItemsByName(List<InventoryItemData> inventoryItemDatas, int leftIndex, int rightIndex)
=======
    public List<UnitCard> SortItemsByName(List<UnitCard> cards, int leftIndex, int rightIndex)
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                InventoryItemData temp = inventoryItemDatas[i];
                inventoryItemDatas[i] = inventoryItemDatas[j];
                inventoryItemDatas[j] = temp;
=======
                UnitCard temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
>>>>>>> Stashed changes
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
