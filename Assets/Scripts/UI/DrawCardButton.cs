using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DrawCardButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{

    public bool CanInteract = true;

    private static GameObject hoveredCard;
    private Vector3 basePosition;
    private Transform playerDrawPile;
    private float scaleFactor = 1.0f;
    private Coroutine hoverCoroutine;
    private bool cardInUse = false;
    private bool direction = false; // true - up, false - down
    //public Image Glow;

    public static DrawCardButton Instance;

    void Awake() {
        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the DrawCardButton singleton");
            Destroy(this);
        }
        else Instance = this;
    }

    void Start() {
        basePosition = UIManager.Instance.PlayerDraw.position;
        playerDrawPile = UIManager.Instance.PlayerDraw;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!CanInteract || DuelManager.Instance.MainDuel.GetStatus(Team.Player).Deck.DrawPileIsEmpty()) return;

        AnimationManager.Instance.StartManaHover(DuelManager.Instance.Settings.DrawCardManaCost, Team.Player);
        Transform cardTransform = null;
        if(!cardInUse) {
            playerDrawPile = UIManager.Instance.PlayerDraw;
            if (playerDrawPile.childCount > 0) {
                cardTransform = playerDrawPile.GetChild(0);
            }
        }
        else {
            if(hoveredCard == null) return;
            cardTransform = hoveredCard.transform;
        }

        if(cardTransform != null && direction == false) {
            if(hoverCoroutine != null) StopCoroutine(hoverCoroutine);
            hoveredCard = cardTransform.gameObject;
            hoveredCard.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            hoveredCard.transform.SetAsLastSibling();
            direction = true;
            cardInUse = true;
            hoverCoroutine = StartCoroutine(HoverSlideCoroutine(hoveredCard.transform, hoverPosition(), true));
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        AnimationManager.Instance.StopManaHover(Team.Player);

        if (hoveredCard == null) return;
        if(hoverCoroutine != null && direction == true) {
            StopCoroutine(hoverCoroutine);
            direction = false;
            hoverCoroutine = StartCoroutine(HoverSlideCoroutine(hoveredCard.transform, basePosition, false));
        }
    }

    public void StopCardHover() {
        cardInUse = false;
        direction = false;  
        hoveredCard = null;
        if(hoverCoroutine != null) StopCoroutine(hoverCoroutine);
    }

    private Vector3 hoverPosition() {
        return new Vector3(
            hoveredCard.transform.position.x,
            hoveredCard.transform.position.y + (15f*UIManager.Instance.MainCanvas.scaleFactor),
            hoveredCard.transform.position.z
        );
    }

    private IEnumerator HoverSlideCoroutine(Transform card, Vector3 targetPos, bool use) {
        float duration = 0.25f;
        yield return AnimationManager.Instance.SimpleTranslate(
            card,
            targetPos,
            duration,
            InterpolationMode.Slerp
        );

        cardInUse = use;
        if(!use) hoveredCard = null;
    }
    /*
    private void RemoveGlowFromDrawPile()
    {
        Transform playerDrawPile = UIManager.Instance.PlayerDraw;
        Image cardBackImage = playerDrawPile.GetComponentInChildren<Image>();
        cardBackImage.color = new Color(0, 0, 0, 0); 
    }
    */
    public void UpdateDrawPileGlow()
    {
        Transform drawPile = UIManager.Instance.PlayerDraw; 
        CharStatus playerStatus = DuelManager.Instance.MainDuel.GetStatus(Team.Player);

        Color newColor = playerStatus.CanDrawCard() ? Color.green : new Color(0, 0, 0, 0);

        Image glowImage = drawPile.GetComponentInChildren<Image>();

        if (glowImage != null)
        {
            glowImage.color = newColor;
            Debug.Log("Glow image color updated.");
        }
        else
        {
            Debug.LogWarning("Glow image not found under PlayerDraw.");
        }
    }
}
