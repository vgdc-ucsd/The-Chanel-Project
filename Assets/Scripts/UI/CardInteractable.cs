using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CIMode
{
    Duel, Inventory, Reward
}

// The MonoBehavior counterpart for a Card, this is what the user actually interacts with
public abstract class CardInteractable : MonoBehaviour,
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerDownHandler,
    IBeginDragHandler, 
    IDragHandler,
    IEndDragHandler
{
    // Reference to the HandInterface
    [HideInInspector] public HandInterface handInterface;

    // fields set through inspector
    public GraphicRaycaster raycaster; 

    // Text fields on the card
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardCost;

    //Image object of the card
    [SerializeField] private Image image;
    [SerializeField] private Color defaultColor, selectedColor;

    // How much the card scales on hover
    private float scaleFactor = 1.1f;

    private static int hoveredCardIndex;
    private Vector3 basePosition;
    private static CardInteractable hoveredCard;

    // Determines if a card is able to be played
    // Not hidden for debugging purposes
    public bool inHand = true;
    public bool CanInteract = true;
    public CIMode mode = CIMode.Duel;
    protected virtual void Awake()
    {
        if(DuelManager.Instance != null) raycaster = DuelManager.Instance.GetComponent<GraphicRaycaster>();
    }

    // Updates the card's text fields with data from card
    public abstract void SetCardInfo();
    public abstract void UpdateCardInfo();

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (mode == CIMode.Inventory) return;
        else if (mode == CIMode.Reward) {
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            return;
        }
        if(inHand && CanInteract) {
            hoveredCard = this;
            hoveredCardIndex = transform.GetSiblingIndex();
            hoveredCard.transform.SetAsLastSibling();
            basePosition = transform.position;
            hoveredCard.transform.position = hoverPosition();
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (mode == CIMode.Inventory) return;
        else if (mode == CIMode.Reward) {
            transform.localScale = Vector3.one;
            return;
        }
        if (inHand) {
            if(this == hoveredCard) {
                transform.position = basePosition;
                hoveredCard.transform.SetSiblingIndex(hoveredCardIndex);
                hoveredCard = null;
            }
            transform.localScale = Vector3.one;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {

    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (mode != CIMode.Duel) return;
        if (inHand && CanInteract) {
            transform.position = eventData.position;
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (mode != CIMode.Duel) return;
        if (inHand && CanInteract) {
            transform.localEulerAngles = Vector3.zero;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (mode != CIMode.Duel) return;
        if (!CanInteract) return;
        if (inHand)
        {
            // Check if the drag ended over a TileInteractable using a raycast
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);
            TileInteractable tile = null;

            foreach (RaycastResult hit in results)
            {
                if (hit.gameObject.GetComponent<TileInteractable>() != null)
                {
                    tile = hit.gameObject.GetComponent<TileInteractable>();
                    break;
                }
            }

            if (tile != null)
            {
                TryPlayCard(tile.location);
            }
            // Reorganize the player's hand
            if (handInterface == null)
            {
                Debug.Log("Could not organize hand, handInterface is uninitialized");
                return;
            }

        }
        handInterface.OrganizeCards();
    }

    public abstract void TryPlayCard(BoardCoords pos);

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            image.color = selectedColor;
        }
        else
        {
            image.color = defaultColor;
        }
    }

    private Vector3 hoverPosition() {
        if (mode != CIMode.Duel) return Vector3.zero;
        return new Vector3(
            transform.position.x, 
            transform.position.y+(50f*UIManager.Instance.MainCanvas.scaleFactor), 
            transform.position.z
        );
    }

    public abstract Card GetCard();
}