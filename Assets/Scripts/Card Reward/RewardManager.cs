using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public Transform CardContainer;
    public Canvas MainCanvas;

    private float heightOffset = -80f;
    private float duration = 0.6f;
    private float delay = 0.2f;

    private List<CardInteractable> cardInteractables;

    public static RewardManager Instance;

    public TMP_Text goldText;
    public TMP_Text incText;

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
            if (ci is UnitCardInteractable) {
                ((UnitCardInteractable)ci).DrawArrows();
            }
            ci.transform.SetParent(cardSlot.transform, false);
            ci.transform.localPosition = new Vector3(0.0f, heightOffset*MainCanvas.scaleFactor, 0.0f);
            ci.mode = CIMode.Reward;
            ci.CanInteract = true;
            cardInteractables.Add(ci);
        }
        goldText.text = PersistentData.Instance.Inventory.Gold.ToString();
        PersistentData.Instance.Inventory.Gold += PersistentData.Instance.CurrentEncounter.RewardGold;
        StartCoroutine(CardAppearAnimation());
        StartCoroutine(IncrementGold());
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
                new Vector3(ci.transform.position.x, heightOffset*MainCanvas.scaleFactor, 0.0f),
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

    private IEnumerator IncrementGold()
    {
        yield return new WaitForSeconds(0.5f);
        goldText.text = PersistentData.Instance.Inventory.Gold.ToString();
        incText.text = "+" + PersistentData.Instance.CurrentEncounter.RewardGold.ToString();
        incText.enabled = true;
        incText.color = Color.yellow;
        float elapsedTime = 0;
        float endTime = 1;
        float startTime = Time.time;
        while (elapsedTime < endTime)
        {
            elapsedTime = Time.time - startTime;
            incText.color = Interpolation.Interpolate(Color.yellow, Color.clear,
                    elapsedTime / endTime, InterpolationMode.Linear);
            yield return null;
        }
        incText.enabled = false;
    }
}
