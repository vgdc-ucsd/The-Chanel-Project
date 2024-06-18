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
    private RectTransform box;

    void Start()
    {
        box = GetComponent<RectTransform>();
    }

    // Determines how much the cards rotate in the player's hand
    private float maxRotationDegrees = 0;
    // Determines how much space is between the cards in the player's hand
    private float cardDistance = 0f;
    // Determines how tall the arch of the cards is in the player's hand
    private float arcIntensity = 0f;

    private List<QueueableAnimation> cardAnimations = new List<QueueableAnimation>();

    // Displays cards neatly in the UI
    public void OrganizeCards()
    {
        cardDistance = 20 * cardObjects.Count * UIManager.Instance.MainCanvas.scaleFactor;

        // clear old animations
        foreach (QueueableAnimation qa in cardAnimations)
        {
            AnimationManager.Instance.StopCoroutine(qa.Animation);
            qa.Animation = null;
        }
        cardAnimations.Clear();

        for (int i = 0; i < cardObjects.Count; i++)
        {
            GameObject card = cardObjects[i].gameObject;

            // Target Position
            float xVal = (float)(1+i)/(cardObjects.Count+1) * box.rect.width * box.localScale.x;
            if(myTeam == Team.Player) xVal -= (box.rect.width * box.localScale.x)/2f;
            xVal *= UIManager.Instance.MainCanvas.scaleFactor;

            Vector3 targetPosition = new Vector3(xVal, 0, 0);
            targetPosition = Quaternion.Euler(new Vector3(0, 0, transform.localEulerAngles.z)) * targetPosition; // rotate
            targetPosition += transform.position;

            // Animation
            if (targetPosition != card.transform.position)
            {
                // new cards
                if (card.transform.parent != this.transform)
                {
                    card.transform.SetParent(this.transform);
                    card.transform.localScale = Vector3.one;
                    //card.transform.localEulerAngles = new Vector3(0, 0, normalizedIndex * maxRotationDegrees);
                    card.transform.localEulerAngles = new Vector3(0, 0, 0);
                    IEnumerator animation = AnimationManager.Instance.SimpleTranslate(
                        card.transform,
                        targetPosition,
                        0.2f,
                        InterpolationMode.Slerp
                    );
                    QueueableAnimation qa = new QueueableAnimation(animation, 0.1f);
                    cardAnimations.Add(qa);
                    AnimationManager.Instance.Enqueue(qa);

                    //FMODUnity.RuntimeManager.PlayOneShot("event:/CardShuffle", transform.position); // SFX
                }
                // old cards
                else
                {
                    IEnumerator animation = AnimationManager.Instance.SimpleTranslate(
                        card.transform,
                        targetPosition,
                        0.2f,
                        InterpolationMode.EaseOut
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
        GlowCards();
    }

    private void GlowCards()
    {
        DrawCardButton.Instance.UpdateDrawPileGlow();
        foreach (GameObject card in cardObjects)
        {
            UnitCardInteractable uci = card.GetComponent<UnitCardInteractable>();
            SpellCardInteractable sci = card.GetComponent<SpellCardInteractable>();

            if (uci != null)
            {
                CheckCard(uci.card, AnimationManager.Instance.CardCanMove, AnimationManager.Instance.CardCantMove);
            }
            else if (sci != null)
            {
                CheckCard(sci.card, AnimationManager.Instance.SpellCardCanUse, AnimationManager.Instance.SpellCardCantUse);
            }
        }
    }

    private void CheckCard<T>(T card, Func<T, IEnumerator> canUseAnimation, Func<T, IEnumerator> cantUseAnimation) where T : Card
    {
        if (card != null && card.ManaCost <= DuelManager.Instance.MainDuel.PlayerStatus.Mana)
        {
            QueueableAnimation qa = new QueueableAnimation(canUseAnimation(card), 0f);
            DuelManager.Instance.MainDuel.Animations.Enqueue(qa);
        }
        else
        {
            QueueableAnimation qa = new QueueableAnimation(cantUseAnimation(card), 0f);
            DuelManager.Instance.MainDuel.Animations.Enqueue(qa);
        }
    }

}