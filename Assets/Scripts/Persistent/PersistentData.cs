using System;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{ 
    public static PersistentData Instance;
    public InventoryData Inventory = new InventoryData();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public Encounter CurrentEncounter;

    [Serializable]
    public class InventoryData
    {
        public List<Card> InactiveCards;
        public List<Card> ActiveCards;

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
    }


}