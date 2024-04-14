using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    // Collection of Cards Player Unlocked
    public Deck cardCollection;
    // Player's Current Deck
    public Deck playerDeck;
    // Player's Gold
    public int playerGold;

    //TEST, Text Reference for Player Gold
    public TextMeshProUGUI goldText;

    // Tracks Index of Cards Generated
    public List<int> shopCards;

    // Prefab ShopCard Template
    public GameObject shopCardPrefab;

    // Inspect GameObject/Screen
    //public GameObject inspectScreen;

    void Start()
    {
        //TESTING VALUE
        playerGold = 1000;

        goldText.text = "Gold: " + playerGold.ToString();
    }

    // Randomly Generates a Card Index from Player's Card Collection
    public int generateNum(int collectionSize)
    {
        int num = Random.Range(0, collectionSize);
        while (shopCards.Contains(num))
        {
            num = Random.Range(0, collectionSize);
        }
        shopCards.Add(num);
        return num;
    }

    
    // OnClick Function from ShopCardInteractable
    public void purchase(Card card)
    {
        if(card.ShopCost <= playerGold)
        {
            //FIX: when changing playerDeck, it permanently affects the ScriptableObject
            playerDeck.addCard(card);
            playerGold -= card.ShopCost;
            goldText.text = "Gold: " + playerGold.ToString();
        }
        else
        {
            //TODO: Set up Proper Response to Insufficient Funds
            Debug.Log("Insufficient Funds");
        }
    }

    // Inspect Function for ShopCardInteractable
    public void inspect(GameObject card)
    {
        Debug.Log("Inspect");
        //inspectScreen.SetActive(true);
    }

    //TODO: REMOVE AFTER TESTING
    /*public void AddItem(ShopItemData shopItem)
    {
        if (shopCards.Count < maxshopCards)
        {
            shopCards.Add(shopItem);
        }
        else
        {
            Debug.Log("No space in shop");
        }

        shopUI.RefreshShopCards();
    }*/

    //TODO: REMOVE AFTER TESTING
    /*public void RemoveItem(Card cardToRemove)
    {
        shopCards.Remove(cardToRemove);

        shopUI.RefreshShopCards();
    }*/

    // TODO: Port Function to Inventory
    // NOT NEEDED FOR SHOP, BUT WONDERFUL FOR INVENTORY :)
    // Quicksort to sort 'shopCards' list by name (in ascending order)

    /*public List<Card> SortCardsByName(List<Card> cards, int leftIndex, int rightIndex)
    {
        var i = leftIndex;
        var j = rightIndex;
        var pivot = cards[leftIndex];

        while (i <= j)
        {
            while (String.Compare(cards[i].name, pivot.name) < 0)
            // cards[i].name < pivot.name)
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
            SortCardsByName(cards, leftIndex, j);

        if (i < rightIndex)
            SortCardsByName(cards, i, rightIndex);

        return cards;
    }*/
}
