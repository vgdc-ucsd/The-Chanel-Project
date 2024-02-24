using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Manages the player's and interactions with their hand
public class HandInterface : MonoBehaviour
{
    public CardInteractable TemplateCard;
    [HideInInspector] public List<GameObject> cardObjects = new List<GameObject>();
    public Team myTeam;

    // Determines how much the cards rotate in the player's hand
    private float maxRotationDegrees = 15;
    // Determines how much space is between the cards in the player's hand
    private float cardDistance = 50f;
    // Determines how tall the arch of the cards is in the player's hand
    private float arcIntensity = 15f;

    // Adds a number of cards to the player's hand

    void Awake() {
        DuelEvents.Instance.OnDrawCard += Draw;
        DuelEvents.Instance.OnRemoveFromHand += RemoveFromHand;
    }

    public void Draw(Card c, Team team) {
        if(TemplateCard == null) {
            Debug.Log("Could not draw cards, TemplateCard is uninitialized");
            return;
        }

        if (team == myTeam) {
            // Draw a random card from the deck (doesn't remove from deck)
            c = ScriptableObject.Instantiate(c);
            c.team = team;
            GameObject cardObject = Instantiate(TemplateCard.gameObject);
            SetCard(c, cardObject);
            cardObject.transform.SetParent(this.transform);
            cardObject.transform.localScale = Vector3.one;
            cardObjects.Add(cardObject);

            OrganizeCards();
        }
    }

    public void RemoveFromHand(Card card)
    {

    }

    // Maps a Card to a CardInteractable
    private void SetCard(Card c, GameObject cardObject) {
        CardInteractable ci = cardObject.GetComponent<CardInteractable>();
        if(ci == null) {
            Debug.Log("Could not set card, TemplateCard has no CardInteractable");
            return;
        }

        ci.card = c;
        ci.handInterface = this;
        ci.SetCardInfo();
    }

    // Displays cards neatly in the UI
    public void OrganizeCards() {
        for(int i = 0; i < cardObjects.Count; i++) {
            GameObject card = cardObjects[i];
            float normalizedIndex = -1 + (2 * (float)i/(cardObjects.Count-1)); // Ranges between -1 and 1
            if(cardObjects.Count == 1) normalizedIndex = 0;
            card.transform.localEulerAngles = new Vector3(0, 0, normalizedIndex * maxRotationDegrees);
            float arcValue = arcIntensity * (1-Mathf.Abs(normalizedIndex)); // Ranges between 0 and arcIntensity
            card.transform.localPosition = new Vector3(-normalizedIndex * cardDistance, arcValue, 0);
            card.transform.SetAsFirstSibling(); // Make sure they appear overlayed in the right order
        }
    }
}
