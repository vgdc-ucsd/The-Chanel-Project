using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverEvent : Event
{
    public Card lancelot;
    public void Help()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        PersistentData.Instance.Inventory.Gold += 100;
        EventManager.Instance.FinishEvent();
    }

    public void DontHelp()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        PersistentData.Instance.Inventory.InactiveCards.Add(lancelot);
        List<Card> addedCards = new()
        {
            lancelot
        };

        StartCoroutine(AnimationManager.Instance.ShowChangedCards(addedCards, center));
    }
}
