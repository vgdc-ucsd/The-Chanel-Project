using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public Transform CardContainer;

    private float heightOffset = -200f;
    private float duration = 0.6f;
    private float delay = 0.2f;

    private List<CardInteractable> cardInteractables;

    public static RewardManager Instance;

    public TMP_Text goldText;

    void Awake() {
        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the RewardManager singleton");
            Destroy(this);
        }
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cardInteractables = new List<CardInteractable>();

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
            ci.CanInteract = true;
            cardInteractables.Add(ci);
        }
        goldText.text = PersistentData.Instance.CurrentEncounter.RewardGold.ToString();
        PersistentData.Instance.Inventory.Gold += PersistentData.Instance.CurrentEncounter.RewardGold;
        StartCoroutine(CardAppearAnimation());
    }

    public void SelectCard(CardInteractable selected) {
        foreach(CardInteractable ci in cardInteractables) {
            ci.CanInteract = false;

            if(ci == selected) {
                PersistentData.Instance.Inventory.InactiveCards.Add(ci.GetCard().Clone());
            }
            else {
                IEnumerator ie = AnimationManager.Instance.SimpleTranslate(
                ci.transform, 
                new Vector3(ci.transform.position.x, heightOffset, 0.0f),
                duration, 
                InterpolationMode.Slerp);
                QueueableAnimation qa = new QueueableAnimation(ie, delay);
                AnimationManager.Instance.Enqueue(qa);
            }
        }

        StartCoroutine(ChangeScene());
    }

    private IEnumerator CardAppearAnimation() {
        yield return null;
        
        foreach(CardInteractable ci in cardInteractables) {
            IEnumerator ie = AnimationManager.Instance.SimpleTranslate(
                ci.transform, 
                ci.transform.parent.position, 
                duration, 
                InterpolationMode.Slerp);
            QueueableAnimation qa = new QueueableAnimation(ie, delay);
            AnimationManager.Instance.Enqueue(qa);
        }
    }

    private IEnumerator ChangeScene() {
        yield return new WaitForSeconds(1.0f);
        MenuScript.Instance.LoadMap();
    }
}
