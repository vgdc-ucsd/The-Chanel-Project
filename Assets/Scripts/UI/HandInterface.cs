using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

// Manages the player's and interactions with their hand
public class HandInterface : MonoBehaviour
{
    [HideInInspector] public List<GameObject> cardObjects = new List<GameObject>();
    public Team myTeam;

    // Determines how much the cards rotate in the player's hand
    private float maxRotationDegrees = 0;
    // Determines how much space is between the cards in the player's hand
    private float cardDistance = 0f;
    // Determines how tall the arch of the cards is in the player's hand
    private float arcIntensity = 0f;

    private List<QueueableAnimation> cardAnimations = new List<QueueableAnimation>(); 

    // Displays cards neatly in the UI
    public void OrganizeCards() {
        cardDistance = 20 * cardObjects.Count * UIManager.Instance.MainCanvas.scaleFactor;

        // clear old animations
        foreach(QueueableAnimation qa in cardAnimations) {
            AnimationManager.Instance.StopCoroutine(qa.Animation);
            qa.Animation = null;
        }
        cardAnimations.Clear();

        for(int i = 0; i < cardObjects.Count; i++) {
            GameObject card = cardObjects[i].gameObject;

            // index
            float normalizedIndex = -1 + (2 * (float)i/(cardObjects.Count-1)); // Ranges between -1 and 1
            if(cardObjects.Count == 1) normalizedIndex = 0;

            // arc
            float arcValue = arcIntensity * (1-Mathf.Abs(normalizedIndex)); // Ranges between 0 and arcIntensity
            
            // Target Position
            Vector3 targetPosition = new Vector3(-normalizedIndex * cardDistance, arcValue, 0);
            targetPosition = Quaternion.Euler(new Vector3(0, 0, transform.localEulerAngles.z)) * targetPosition; // rotate
            targetPosition += transform.position;

            // Animation
            if(targetPosition != card.transform.position) {
                // new cards
                if(card.transform.parent != this.transform) {
                    card.transform.SetParent(this.transform);
                    card.transform.localScale = Vector3.one;
                    card.transform.localEulerAngles = new Vector3(0, 0, normalizedIndex * maxRotationDegrees);
                    IEnumerator animation = AnimationManager.Instance.SimpleTranslate(
                        card.transform,
                        targetPosition,
                        0.2f,
                        InterpolationMode.Linear
                    );
                    QueueableAnimation qa = new QueueableAnimation(animation, 0.1f);
                    cardAnimations.Add(qa);
                    AnimationManager.Instance.Enqueue(qa);
                }
                // old cards
                else {
                    IEnumerator animation = AnimationManager.Instance.SimpleTranslate(
                        card.transform,
                        targetPosition,
                        0.1f,
                        InterpolationMode.Linear
                    );
                    QueueableAnimation qa = new QueueableAnimation(animation, 0f);
                    cardAnimations.Add(qa);
                    AnimationManager.Instance.Play(qa.Animation); // enqueue?
                    //AnimationManager.Instance.Enqueue(qa); // enqueue?
                }   
            }

            //card.transform.position = Vector3.zero;

            // Make sure they appear overlayed in the right order
            card.transform.SetAsFirstSibling(); 
        }
    }
}
