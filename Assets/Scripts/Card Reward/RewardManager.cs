using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public Transform CardContainer;

    private float heightOffset = -200f;
    private float duration = 0.6f;
    private float delay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Card c in PersistentData.Instance.CurrentEncounter.CardOffers) {
            GameObject cardSlot = new GameObject{ name = "card slot" };
            cardSlot.AddComponent<RectTransform>();
            cardSlot.transform.SetParent(CardContainer, false);

            Card reward = c.Clone();
            c.CurrentTeam = Team.Neutral;
            CardInteractable ci = UIManager.Instance.GenerateCardInteractable(c);
            ci.transform.SetParent(cardSlot.transform, false);
            ci.transform.localPosition = new Vector3(0.0f, heightOffset, 0.0f);
            ci.mode = CIMode.Reward;
        }

        StartCoroutine(CardAppearAnimation());
    }

    private IEnumerator CardAppearAnimation() {
        yield return null;
        
        foreach(Transform obj in CardContainer) {
            IEnumerator ie = AnimationManager.Instance.SimpleTranslate(
                obj.GetChild(0).transform, 
                obj.transform.position, 
                duration, 
                InterpolationMode.Slerp);
            QueueableAnimation qa = new QueueableAnimation(ie, delay);
            AnimationManager.Instance.Enqueue(qa);
        }
    }
}
