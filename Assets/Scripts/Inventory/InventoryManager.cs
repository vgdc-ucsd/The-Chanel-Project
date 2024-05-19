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
    public int maxItems = 10; // Testing
    public InventoryUI inventoryUI;

    public Deck playerDeck;
    public Deck playerInventoryDeck;
    public List<UnitCard> cards;

    void Start()
    {
        // Create a new list at the start
        InitializeInventory();

        // NOTE: There should only be one object with InventoryUI scripts. If not
        // this line does not work properly.
        inventoryUI = FindObjectOfType<InventoryUI>();

        // Adds inventory cards to list
        
        // Use a deck as a list of all inventory cards, should be no max card limit as we wont reach it
    }

    private void InitializeInventory()
    {
        cards = new List<UnitCard>(maxItems);
        //Debug.Log(playerInventoryDeck.CardList);
        foreach (Card card in playerInventoryDeck.CardList)
        {
            cards.Add((UnitCard)card); // idk if this cast works lol
        }
        inventoryUI.RefreshInventoryItems();
    }

    public void DisplayCards()
    {
        if (cards.Count < maxItems)
        {
            //cards.Add(card);
        }
        else
        {
            Debug.Log("No space in inventory");
        }


        inventoryUI.RefreshInventoryItems();
    }


    public void RemoveItem(UnitCard card)
    {
        cards.Remove(card);

        inventoryUI.RefreshInventoryItems();
    }

    // Quicksort to sort 'items' list by name (in ascending order)
    public List<UnitCard> SortItemsByName(List<UnitCard> cards, int leftIndex, int rightIndex)
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
                UnitCard temp = cards[i];
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
