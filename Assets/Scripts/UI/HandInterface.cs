using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

// Manages the player's and interactions with their hand
public class HandInterface : MonoBehaviour
{
    [HideInInspector] public List<CardInteractable> cardObjects = new List<CardInteractable>();
    public Team myTeam;

    // Determines how much the cards rotate in the player's hand
    private float maxRotationDegrees = 15;
    // Determines how much space is between the cards in the player's hand
    private float cardDistance = 0f;
    // Determines how tall the arch of the cards is in the player's hand
    private float arcIntensity = 30f;

    private List<QueueableAnimation> cardAnimations = new List<QueueableAnimation>(); 

    void Awake() {
        DuelEvents.Instance.OnDrawCard += Draw;
        DuelEvents.Instance.OnRemoveFromHand += RemoveFromHand;
        DuelEvents.Instance.onUpdateHand += OrganizeCards;
    }

    public void Draw(Card c, Team team) {
        if (team == myTeam) {
            // Draw a random card from the deck (doesn't remove from deck) 
            c.CurrentTeam = team; // ?
            CardInteractable ci = DuelManager.Instance.UI.GenerateCardInteractable(c);
            ci.handInterface = this;
            ci.transform.localScale = Vector3.one;
            cardObjects.Add(ci);
        }
    }

    public void RemoveFromHand(Card card)
    {
        //Debug.Log("here");
        //cardObjects.Remove(card.CardInteractableRef.gameObject);
        //foreach(CardInteractable ci in cardObjects) {
        //    if(ci.card == card) {
        //        Debug.Log("here");
        //        cardObjects.Remove(ci);
        //    }
        //}
    }

    // Displays cards neatly in the UI
    public void OrganizeCards() {
        cardDistance = 45 * cardObjects.Count;

        // clear old animations
        foreach(QueueableAnimation qa in cardAnimations) {
            DuelManager.Instance.AM.StopCoroutine(qa.Animation);
            qa.Animation = null;
        }
        cardAnimations.Clear();

        for(int i = 0; i < cardObjects.Count; i++) {
            GameObject card = cardObjects[i].gameObject;

            // index
            float normalizedIndex = -1 + (2 * (float)i/(cardObjects.Count-1)); // Ranges between -1 and 1
            if(cardObjects.Count == 1) normalizedIndex = 0;

            // arc
            card.transform.localEulerAngles = new Vector3(0, 0, normalizedIndex * maxRotationDegrees);
            float arcValue = arcIntensity * (1-Mathf.Abs(normalizedIndex)); // Ranges between 0 and arcIntensity
            
            // Target Position
            Vector3 targetPosition = new Vector3(-normalizedIndex * cardDistance, arcValue, 0);
            targetPosition += transform.position;

            // Animation
            if(targetPosition != card.transform.position) {
                // new cards
                if(card.transform.parent != this.transform) {
                    card.transform.SetParent(this.transform);
                    card.transform.localScale = Vector3.one;
                    IEnumerator animation = DuelManager.Instance.AM.SimpleTranslate(
                        card.transform,
                        targetPosition,
                        0.2f,
                        InterpolationMode.Linear
                    );
                    QueueableAnimation qa = new QueueableAnimation(animation, 0.1f);
                    cardAnimations.Add(qa);
                    DuelManager.Instance.AM.Enqueue(qa);
                }
                // old cards
                else {
                    IEnumerator animation = DuelManager.Instance.AM.SimpleTranslate(
                        card.transform,
                        targetPosition,
                        0.1f,
                        InterpolationMode.Linear
                    );
                    QueueableAnimation qa = new QueueableAnimation(animation, 0f);
                    cardAnimations.Add(qa);
                    DuelManager.Instance.AM.Play(qa.Animation);
                }   
            }

            // Make sure they appear overlayed in the right order
            card.transform.SetAsFirstSibling(); 
        }
    }
}
