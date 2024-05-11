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
    public GameObject inspectScreen;

    void Start()
    {
        //TESTING VALUE
        playerGold = 4;

        goldText.text = "Gold: " + playerGold.ToString();
    }

    // TESTING ONLY, REMOVE LATER
    void OnApplicationQuit()
    {
        Debug.Log("QUIT: RESETTING TEST DECK");
        playerDeck.CardList.Clear();
    }

    // Randomly Generates a Card Index from Player's Card Collection
    public int generateNum(int collectionSize)
    {
        Debug.Log("CollectionSize: " + collectionSize);
        int num = Random.Range(0, collectionSize);
        while (shopCards.Contains(num))
        {
            num = Random.Range(0, collectionSize);
        }
        shopCards.Add(num);
        return num;
    }

    
    // OnClick Function from ShopCardInteractable
    public bool purchase(Card card)
    {
        if(card.ShopCost <= playerGold)
        {
            //TODO FIX: when changing playerDeck, it permanently affects the ScriptableObject
            playerDeck.addCard(card);
            playerGold -= card.ShopCost;
            goldText.text = "Gold: " + playerGold.ToString();
            return true;
        }
        else
        {
            //TODO: Set up Proper Response to Insufficient Funds
            Debug.Log("Insufficient Funds");
            return false;
        }
    }

    // Inspect Function for ShopCardInteractable
    public void inspect(Card card)
    {
        inspectScreen.SetActive(true);
        // Gets GameObject Reference of Card
        GameObject inspectCard = inspectScreen.transform.GetChild(0).gameObject;

        CardDisplay display = inspectCard.GetComponent<CardDisplay>();
        display.setDisplay(card);
    }

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
