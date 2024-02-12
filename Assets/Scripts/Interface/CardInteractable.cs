using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// The MonoBehavior counterpart for a Card, this is what the user actually interacts with
public class CardInteractable : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IBeginDragHandler, 
    IEndDragHandler,
    IDragHandler
{


    // Data for the card it contains
    [HideInInspector] public Card card;

    // Reference to the HandInterface
    [HideInInspector] public HandInterface handInterface;

    // fields set through inspector
    public DuelManager duelManager; 
    public GraphicRaycaster raycaster; 
    public GameObject TemplateArrowPlayer;
    public GameObject TemplateArrowEnemy;

    // Text fields on the card
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;

    // How much the card scales on hover
    private float scaleFactor = 1.1f;

    // Determines if a card is able to be played
    // Not hidden for debugging purposes
    public bool inHand = true;

    public void Awake()
    {
        DuelEvents.instance.onUpdateUI += UpdateCardInfo;
    }

    public void DrawArrows() {
        foreach(Vector2Int v in card.AttackDirections) {
            GameObject arrow;
            if(card.BelongsToPlayer) {
                arrow = Instantiate(TemplateArrowPlayer);
            }
            else {
                arrow = Instantiate(TemplateArrowEnemy);
            }
            
            arrow.transform.localScale = new Vector3(
                arrow.transform.localScale.x,
                v.magnitude/2,
                1
            );
            arrow.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, v));
            arrow.transform.SetParent(this.transform);
            arrow.transform.localPosition = Vector3.zero;
            arrow.SetActive(true);
        }
    }

    // Updates the card's text fields with data from card
    public void SetCardInfo() {
        if(card == null) {
            Debug.Log("Could not set card info, card is uninitialzied");
            return;
        }
        CardName.text = card.Name;

        card.CardInteractableRef = this;
        UpdateCardInfo();
    }

    public void UpdateCardInfo() 
    {
        CardAttack.text = "Attack: " + card.Attack;
        CardHealth.text = "Health: " + card.Health;
    }

    // Updates UI to show card being played
    public void PlaceCard(TileInteractable tile) {
        if (tile != null && tile.occupied == false) {
            inHand = false;
            tile.occupied = true;
            card.TileInteractableRef = tile;
            transform.position = tile.transform.position;
            if(handInterface != null) handInterface.cards.Remove(this.gameObject);
            transform.SetParent(tile.transform);
            transform.localScale = Vector3.one;
            DrawArrows();
            duelManager.PlayCard(card, tile.location.x, tile.location.y); // x is row y is col 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(inHand) {
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(inHand) {
            transform.localScale = Vector3.one;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(inHand) {
            transform.position = eventData.position;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(inHand) {
            transform.localEulerAngles = Vector3.zero;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(inHand) {
            // Check if the drag ended over a TileInteractable using a raycast
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);
            TileInteractable tile = null;

            foreach(RaycastResult hit in results) {
                if(hit.gameObject.GetComponent<TileInteractable>() != null) {
                    tile = hit.gameObject.GetComponent<TileInteractable>();
                    break;
                }
            }

            if(tile != null && tile.location.x >= 1) { // can't place in the row closest to enemy
                PlaceCard(tile);
            }

            // Reorganize the player's hand
            if(handInterface == null) {
                Debug.Log("Could not organize hand, handInterface is uninitialized");
                return;
            }
            handInterface.OrganizeCards();
        }
    }
}
