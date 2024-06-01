using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeraEvent : Event
{
    private List<Card> removedCards = new List<Card>();

    public void Listen()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        List<int> cardsWithAbilities = new List<int>();
        for (int i = 0; i < PersistentData.Instance.Inventory.ActiveCards.Count; i++)
        {
            Card c = PersistentData.Instance.Inventory.ActiveCards[i];
            if (c is UnitCard)
            {
                UnitCard uc = (UnitCard)c;
                if (uc.Abilities.Count > 0) cardsWithAbilities.Add(i);
            }
        }

        if (cardsWithAbilities.Count > 0 && PersistentData.Instance.Inventory.InactiveCards.Count > 0)
        {
            List<Card> removedCards = new List<Card>();
            int randomIndex = Random.Range(0, cardsWithAbilities.Count);
            Card removed = PersistentData.Instance.Inventory.ActiveCards[cardsWithAbilities[randomIndex]];
            removedCards.Add(removed);
            PersistentData.Instance.Inventory.ActiveCards.Remove(removed);
            StartCoroutine(AnimationManager.Instance.ShowChangedCards(removedCards, center, MenuScript.MAP_INDEX));
        }
        else
        {
            EventManager.Instance.FinishEvent();
        }
    }

    public void Dodge()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        PersistentData.Instance.Inventory.Gold = 0;
        // TODO sound effect or animation

        EventManager.Instance.FinishEvent();
    }
}
