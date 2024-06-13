using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEvent : Event
{
    public void Option1()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        List<Card> strongCards = GameData.Instance.GetCardsOfType(CardType.Strong);

        Card randomStrongCard = strongCards[Random.Range(0, strongCards.Count)];
        PersistentData.Instance.Inventory.InactiveCards.Add(randomStrongCard.Clone());
        List<Card> addedCards = new List<Card> { randomStrongCard };
        StartCoroutine(AnimationManager.Instance.ShowChangedCards(addedCards, center));
    }

    public void Option2()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        List<Card> possibleCards = new List<Card>();
        possibleCards.AddRange(PersistentData.Instance.Inventory.ActiveCards);
        possibleCards.AddRange(PersistentData.Instance.Inventory.InactiveCards);
        List<Card> addedCards = new List<Card>();
        Card card1 = possibleCards[Random.Range(0, possibleCards.Count)];
        addedCards.Add(card1);
        Card card2 = possibleCards[Random.Range(0, possibleCards.Count)];
        int i = 0;
        while (card1.Name == card2.Name)
        {
            card2 = possibleCards[Random.Range(0, possibleCards.Count)];
            i++;
            if (i > 100)
            {
                Debug.LogError("Failed to generate duplicates");
                return;
            }
        }
        addedCards.Add(card2);
        foreach (Card card in addedCards)
        {
            PersistentData.Instance.Inventory.InactiveCards.Add(card.Clone());
        }
        StartCoroutine(AnimationManager.Instance.ShowChangedCards(addedCards, center));
    }
}
