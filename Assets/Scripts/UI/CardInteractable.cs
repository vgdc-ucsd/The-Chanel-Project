using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// The MonoBehavior counterpart for a Card, this is what the user actually interacts with
public abstract class CardInteractable : MonoBehaviour,
    IPointerEnterHandler, 
    IPointerExitHandler, 
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

    public void Awake()
    {
        if(DuelManager.Instance != null) raycaster = DuelManager.Instance.GetComponent<GraphicRaycaster>();
    }

    // Updates the card's text fields with data from card
    public abstract void SetCardInfo();
    public abstract void UpdateCardInfo();

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if(inHand) {
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
        if(inHand) {
            if(this == hoveredCard) {
                transform.position = basePosition;
                hoveredCard.transform.SetSiblingIndex(hoveredCardIndex);
                hoveredCard = null;
            }
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
        return new Vector3(
            transform.position.x, 
            transform.position.y+(50f*UIManager.Instance.MainCanvas.scaleFactor), 
            transform.position.z
        );
    }
}