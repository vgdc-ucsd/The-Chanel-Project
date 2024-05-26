using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public UnitCardInteractable UCITemplate;
    public SpellCardInteractable SCITemplate;

    public List<Card> AllCards = new List<Card>();

    public static int DECK_SIZE = 12;

    public const int BASE_GOLD = 100;
    public const int GOLD_SCALING = 20; // how much gold reward increases after every duel
    public const float GOLD_VARIANCE = 0.2f; // randomness in gold reward

    // type of card start appearing after how many duels played (0-indexed)
    public const int MED_CARDS_CUTOFF = 2;
    public const int STRONG_CARDS_CUTOFF = 4;

    public const int CARD_REWARD_CHOICES = 3;
    public const float SPELLCARD_REWARD_CHANCE = 0.1f; // (rough) chance of rewards containing 1 spellcard, actual chance
                                                       // is slightly less

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

    public List<Card> GetCardsOfTypes(List<CardType> cardTypes)
    {
        List<Card> cards = new List<Card>();
        foreach (Card card in AllCards)
        {
            if (cardTypes.Contains(card.cardType))
            {
                cards.Add(card);
            }
        }
        return cards;
    }

    public List<Card> GetCardsOfType(CardType cardType)
    {
        List<Card> cards = new List<Card>();
        foreach (Card card in AllCards)
        {
            if (card.cardType == cardType)
            {
                cards.Add(card);
            }
        }
        return cards;
    }

}