using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;
    public InventoryData Inventory = new InventoryData();
    public MapInfo mapInfo;

    public Deck ImportDeck;
    public int EncountersFinished = 0;
    public List<Encounter> possibleEncounters;
    public List<Encounter> completedEncounters;
    public List<GameObject> PossibleEvents;
    public List<GameObject> CompletedEvents;
    public int HealthOverride = -1;
    public VsScreenState VsState;

    public List<Card> ShopOffers = new List<Card>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);


        Init();

    }

    public Encounter CurrentEncounter;

    [Serializable]
    public class InventoryData
    {
        public List<Card> InactiveCards = new List<Card>();
        public List<Card> ActiveCards = new List<Card>();

        public int Gold = 0;
        public InventoryData()
        {
            InactiveCards = new List<Card>();
            ActiveCards = new List<Card>();
            Gold = 0;
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

    public void Init()
    {
        Inventory = new InventoryData();
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
        else // make duplicates unique cards
        {
            for (int i = 0; i < Inventory.ActiveCards.Count; i++)
            {
                Card card = Inventory.ActiveCards[0].Clone();
                Inventory.ActiveCards.RemoveAt(0);
                Inventory.ActiveCards.Add(card);
            }
            for (int i = 0; i < Inventory.InactiveCards.Count; i++)
            {
                Card card = Inventory.InactiveCards[0].Clone();
                Inventory.InactiveCards.RemoveAt(0);
                Inventory.InactiveCards.Add(card);
            }
        }

        EncountersFinished = 0;
    }



    /*
     * Sets the variable encounter data (rewards, difficulty etc) immediately before
     * starting the next encounter
     */
    public void SetEncounterStats()
    {
        if (possibleEncounters.Count > 0)
        {
            CurrentEncounter = possibleEncounters[UnityEngine.Random.Range(0, possibleEncounters.Count - 1)];
            possibleEncounters.Remove(CurrentEncounter);
            completedEncounters.Add(CurrentEncounter);
        }
        else if (completedEncounters.Count > 0)
        {
            CurrentEncounter = completedEncounters[UnityEngine.Random.Range(0, completedEncounters.Count - 1)];
        }

        // Choose deck of strength 1 to 3 if not preset, difficulty depends on encounters finished
        int deckIndex;
        if (EncountersFinished < 1) deckIndex = 0;
        else if (EncountersFinished < 3) deckIndex = 1;
        else deckIndex = 2;
        Debug.Log("Deck chosen: " + deckIndex + " on encounter: " + EncountersFinished);
        CurrentEncounter.EnemyDeck = CurrentEncounter.EnemyDecks[deckIndex];

        // Randomize gold reward
        CurrentEncounter.RewardGold = (int)((GameData.BASE_GOLD + EncountersFinished * GameData.GOLD_SCALING)
                                        * UnityEngine.Random.Range(1 - GameData.GOLD_VARIANCE, 1 + GameData.GOLD_VARIANCE));

        // Randomize card reward based on progression
        List<Card> rewardPool;
        if (EncountersFinished < GameData.MED_CARDS_CUTOFF)
            rewardPool = GameData.Instance.GetCardsOfTypes(new List<CardType> { CardType.Weak });
        else if (EncountersFinished < GameData.STRONG_CARDS_CUTOFF)
            rewardPool = GameData.Instance.GetCardsOfTypes(new List<CardType> { CardType.Weak, CardType.Medium });
        else
            rewardPool = GameData.Instance.GetCardsOfTypes(new List<CardType> { CardType.Medium, CardType.Strong });

        // randomly add spell cards to potential reward pool
        if (UnityEngine.Random.value < GameData.SPELLCARD_REWARD_CHANCE)
        {
            rewardPool.AddRange(GameData.Instance.GetCardsOfType(CardType.Spell));
        }

        List<Card> rewardCards = new List<Card>();
        int i = 0;
        int iter = 0;
        bool spellAdded = false;
        while (i < GameData.CARD_REWARD_CHOICES)
        {
            Card cardToAdd = rewardPool[UnityEngine.Random.Range(0, rewardPool.Count)];
            if (!rewardCards.Contains(cardToAdd) &&
                !(spellAdded && cardToAdd.cardType == CardType.Spell)) // prevent more than 1 spell card per reward
            {
                if (cardToAdd.cardType == CardType.Spell) spellAdded = true;
                rewardCards.Add(cardToAdd);
                i++;
            }
            iter++;
            if (iter > 100)
            {
                Debug.LogError("Failed to generate rewards");
                break;
            }
        }

        CurrentEncounter.CardOffers = rewardCards.ToArray();
    }

    public void SetNextEncounter(Encounter encounter)
    {
        CurrentEncounter = encounter;
        // todo: track what encounters already completed
    }

    public void GenerateShopOffers()
    {
        // Randomize card reward based on progression
        List<Card> rewardPool;
        if (EncountersFinished < GameData.MED_CARDS_CUTOFF)
            rewardPool = GameData.Instance.GetCardsOfTypes(new CardType[] { CardType.Weak }.ToList());
        else if (EncountersFinished < GameData.STRONG_CARDS_CUTOFF)
            rewardPool = GameData.Instance.GetCardsOfTypes(new CardType[] { CardType.Weak, CardType.Medium }.ToList());
        else
            rewardPool = GameData.Instance.GetCardsOfTypes(new CardType[] { CardType.Medium, CardType.Strong }.ToList());

        rewardPool.AddRange(GameData.Instance.GetCardsOfType(CardType.Spell));

        List<Card> rewardCards = new List<Card>();
        int i = 0;
        int iter = 0;
        bool spellAdded = false;
        while (i < 5)
        {
            Card cardToAdd = rewardPool[UnityEngine.Random.Range(0, rewardPool.Count)];
            if (!rewardCards.Contains(cardToAdd) &&
                !(spellAdded && cardToAdd.cardType == CardType.Spell)) // prevent more than 1 spell card per reward
            {
                if (cardToAdd.cardType == CardType.Spell) spellAdded = true;
                rewardCards.Add(cardToAdd);
                i++;
            }
            iter++;
            if (iter > 100)
            {
                Debug.LogError("Failed to generate rewards");
                break;
            }
        }

        ShopOffers = rewardCards;
    }

}