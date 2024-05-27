using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClioEvent : MonoBehaviour
{
    public Transform center;

    private List<Card> removedCards = new List<Card>();
    private float heightOffset = 300f;
    private float duration = 0.6f;

    public void HelpOut() {
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;

        int cardCount = PersistentData.Instance.Inventory.InactiveCards.Count;
        if(cardCount > 3) cardCount = 3;

        for(int i = 0; i < cardCount; i++) {
            int randomIndex = Random.Range(0, PersistentData.Instance.Inventory.InactiveCards.Count);
            removedCards.Add(PersistentData.Instance.Inventory.InactiveCards[randomIndex]);
            PersistentData.Instance.Inventory.InactiveCards.RemoveAt(randomIndex);
        }

        StartCoroutine(AnimationManager.Instance.ShowChangedCards(removedCards, center));
    }

    public void Misery() {
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        Debug.Log("misery");

        // TODO


        EventManager.Instance.FinishEvent();
    }
}
