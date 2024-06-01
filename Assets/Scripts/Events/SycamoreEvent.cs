using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SycamoreEvent : Event
{
    public Card rewardCard1Template;
    public Card rewardCard2Template;

    public void PickUp()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        PersistentData.Instance.Inventory.InactiveCards.Add(rewardCard1Template.Clone());
        StartCoroutine(AnimationManager.Instance.ShowChangedCards(
            new List<Card> { rewardCard1Template }, center));
    }

    public void Fight()
    {
        if (EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        FightCharacter();

        PersistentData.Instance.Inventory.InactiveCards.Add(rewardCard2Template.Clone());
        StartCoroutine(AnimationManager.Instance.ShowChangedCards(
            new List<Card> { rewardCard2Template }, center, MenuScript.VERSUS_INDEX));

    }
}
