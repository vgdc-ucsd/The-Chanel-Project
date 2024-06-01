using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnyxEvent : Event
{
    private List<Card> removedCards = new List<Card>();
    private float heightOffset = 300f;
    private float duration = 0.6f;

    public void DoNothing() {
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        int gold = PersistentData.Instance.Inventory.Gold;
        gold -= 100;
        if(gold < 0) gold = 0;
        PersistentData.Instance.Inventory.Gold = gold;

        List<UnitCard> ActiveUnitCards = new List<UnitCard>();
        List<UnitCard> InactiveUnitCards = new List<UnitCard>();
        foreach(Card c in PersistentData.Instance.Inventory.ActiveCards) {
            if(c is UnitCard uc) ActiveUnitCards.Add(uc);
        }
        foreach(Card c in PersistentData.Instance.Inventory.InactiveCards) {
            if(c is UnitCard uc) InactiveUnitCards.Add(uc);
        }

        List<UnitCard> cardList;
        if(ActiveUnitCards.Count > 0 && InactiveUnitCards.Count > 0) {
            int randomList = Random.Range(0, 2);
            if(randomList == 0) {
                cardList = ActiveUnitCards;
            }
            else {
                cardList = InactiveUnitCards;
            }
        }
        else if(ActiveUnitCards.Count > 0) {
            cardList = ActiveUnitCards;
        }
        else if(InactiveUnitCards.Count > 0) {
            cardList = InactiveUnitCards;
        }
        else {
            EventManager.Instance.FinishEvent();
            return;
        }

        int randomIndex = Random.Range(0, cardList.Count);
        UnitCard selectedCard = cardList[randomIndex];
        selectedCard.Health += 3;
        List<Card> changedCards = new List<Card>();
        changedCards.Add(selectedCard);
        StartCoroutine(AnimationManager.Instance.ShowChangedCards(changedCards, center));
    }

    public void Rotate() {
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        AcknowledgeCharacter();

        int gold = PersistentData.Instance.Inventory.Gold;
        gold -= 100;
        if(gold < 0) gold = 0;
        PersistentData.Instance.Inventory.Gold = gold;

        List<UnitCard> ActiveUnitCards = new List<UnitCard>();
        List<UnitCard> InactiveUnitCards = new List<UnitCard>();
        foreach(Card c in PersistentData.Instance.Inventory.ActiveCards) {
            if(c is UnitCard uc) ActiveUnitCards.Add(uc);
        }
        foreach(Card c in PersistentData.Instance.Inventory.InactiveCards) {
            if(c is UnitCard uc) InactiveUnitCards.Add(uc);
        }

        if(ActiveUnitCards.Count + InactiveUnitCards.Count <= 0) {
            EventManager.Instance.FinishEvent();
            return;
        }

        List<UnitCard> cardList;
        List<Card> changedCards = new List<Card>();
        for(int i = 0; i < 3; i++) {
            if(ActiveUnitCards.Count > 0 && InactiveUnitCards.Count > 0) {
            int randomList = Random.Range(0, 2);
            if(randomList == 0) {
                cardList = ActiveUnitCards;
            }
            else {
                cardList = InactiveUnitCards;
            }
            }
            else if(ActiveUnitCards.Count > 0) {
                cardList = ActiveUnitCards;
            }
            else if(InactiveUnitCards.Count > 0) {
                cardList = InactiveUnitCards;
            }
            else {
                continue;
            }

            int randomIndex = Random.Range(0, cardList.Count);
            UnitCard selectedCard = cardList[randomIndex];
            cardList.Remove(selectedCard);
            selectedCard.Health++;
            changedCards.Add(selectedCard);
        }

        StartCoroutine(AnimationManager.Instance.ShowChangedCards(changedCards, center));
    }
}
