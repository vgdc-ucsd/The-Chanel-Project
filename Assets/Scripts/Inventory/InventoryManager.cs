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
    // IGNORE THIS maxItems. IT IS FOR TESTING PURPOSES
    public int maxItems = 10;
    // IDK IF THIS IS SUPPOSED TO BE A LIST OF CARDS OR A DECK
    public List<Card> cards;
    public InventoryUI inventoryUI;

    void Start()
    {
        // Create a new list at the start
        InitializeInventory();

        // NOTE: There should only be one object with InventoryUI scripts. If not
        // this line does not work properly.
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    private void InitializeInventory()
    {
        cards = new List<Card>(maxItems);
    }

    public void AddItem(Card card)
    {
        if (cards.Count < maxItems)
        {
            cards.Add(card);
        }
        else
        {
            Debug.Log("No space in inventory");
        }

        inventoryUI.RefreshInventoryItems();
    }

    public void RemoveItem(Card card)
    {
        cards.Remove(card);

        inventoryUI.RefreshInventoryItems();
    }

    // Quicksort to sort 'items' list by name (in ascending order)
    public List<Card> SortItemsByName(List<Card> cards, int leftIndex, int rightIndex)
    {
        var i = leftIndex;
        var j = rightIndex;
        var pivot = cards[leftIndex];

        while (i <= j)
        {
            while (String.Compare(cards[i].name, pivot.name) < 0)
            // inventoryItemDatas[i].name < pivot.name)
            {
                i++;
            }

            while (String.Compare(cards[j].name, pivot.name) > 0)
            {
                j--;
            }

            if (i <= j)
            {
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
                i++;
                j--;
            }
        }

        if (leftIndex < j)
            SortItemsByName(cards, leftIndex, j);

        if (i < rightIndex)
            SortItemsByName(cards, i, rightIndex);

        return cards;
    }
}
