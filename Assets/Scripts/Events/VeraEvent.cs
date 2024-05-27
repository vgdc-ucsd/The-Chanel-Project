using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeraEvent : MonoBehaviour
{
    public Transform center;

    private List<Card> removedCards = new List<Card>();
    private float heightOffset = 300f;
    private float duration = 0.6f;

    public void Listen()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;

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

        if (cardsWithAbilities.Count > 0)
        {
            List<Card> removedCards = new List<Card>();
            int randomIndex = Random.Range(0, cardsWithAbilities.Count);
            Card removed = PersistentData.Instance.Inventory.ActiveCards[cardsWithAbilities[randomIndex]];
            removedCards.Add(removed);
            PersistentData.Instance.Inventory.ActiveCards.RemoveAt(cardsWithAbilities[randomIndex]);
            StartCoroutine(AnimationManager.Instance.ShowChangedCards(removedCards, center));
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

        PersistentData.Instance.Inventory.Gold = 0;
        // TODO sound effect or animation

        EventManager.Instance.FinishEvent();
    }
}
