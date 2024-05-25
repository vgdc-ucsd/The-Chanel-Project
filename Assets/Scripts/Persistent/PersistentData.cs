using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;
    public InventoryData Inventory = new InventoryData();
    public MapInfo mapInfo;

    public Deck ImportDeck;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        if (ImportDeck != null)
        {
            Deck newDeck = ImportDeck.Clone();
            newDeck.Init();
            Inventory.InactiveCards.Clear();
            Inventory.ActiveCards.Clear();
            foreach (Card card in newDeck.CardList)
            {
                if (Inventory.ActiveCards.Count < Deck.DECK_SIZE)
                {
                    Inventory.ActiveCards.Add(card);
                }
                else Inventory.InactiveCards.Add(card);
            }
        }

    }

    public Encounter CurrentEncounter;

    [Serializable]
    public class InventoryData
    {
        public List<Card> InactiveCards = new List<Card>();
        public List<Card> ActiveCards = new List<Card>();

        public InventoryData()
        {
            InactiveCards = new List<Card>();
            ActiveCards = new List<Card>();
        }

        public int CardCount()
        {
            return InactiveCards.Count + ActiveCards.Count;
        }

        public void Remove(Card card)
        {
            if (InactiveCards.Remove(card)) return;
            if (ActiveCards.Remove(card)) return;
            Debug.LogError("Failed to remove card");
        }

        public bool IsActive(Card card)
        {
            if (ActiveCards.Contains(card)) return true;
            if (InactiveCards.Contains(card)) return false;
            Debug.LogError("Inventory does not contain card");
            return false;
        }
    }


}