using System;
using System.Collections.Generic;
using UnityEngine;

public class OutlawEvent : MonoBehaviour
{
    public Transform Center;
    public List<Encounter> OutlawEncounters;

    private int encounterIndex;

    public void Reach() { // + 3 hp on random card
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;

        List<UnitCard> UnitCards = new List<UnitCard>();
        foreach(Card c in PersistentData.Instance.Inventory.ActiveCards) {
            if(c is UnitCard uc) UnitCards.Add(uc);
        }
        foreach(Card c in PersistentData.Instance.Inventory.InactiveCards) {
            if(c is UnitCard uc) UnitCards.Add(uc);
        }

        if(UnitCards.Count > 0) {
            int randomIndex = UnityEngine.Random.Range(0, UnitCards.Count);
            UnitCard uc = UnitCards[randomIndex];
            uc.Health += 3;
            List<Card> changedCards = new List<Card>{uc};
            StartCoroutine(AnimationManager.Instance.ShowChangedCards(changedCards, Center));
        }
        else {
            EventManager.Instance.FinishEvent();
        }
    }

    public void PitchFork() { // challenge outlaw to duel and get powerful card on win
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;

        int randomIndex = UnityEngine.Random.Range(0, OutlawEncounters.Count);
        PersistentData.Instance.CurrentEncounter = OutlawEncounters[randomIndex];

        PersistentData.Instance.CurrentEncounter = OutlawEncounters[Math.Min(OutlawEncounters.Count - 1, encounterIndex++)];
        EventManager.Instance.FinishEvent(MenuScript.DUEL_INDEX);
    }
}
