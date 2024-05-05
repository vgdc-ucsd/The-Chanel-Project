using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// The MonoBehavior counterpart for a Card, this is what the user actually interacts with
public abstract class CardInteractable : MonoBehaviour,
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IBeginDragHandler, 
    IDragHandler
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
    private static CardInteractable hoveredCard;

    // Determines if a card is able to be played
    // Not hidden for debugging purposes
    public bool inHand = true;

    public void Awake()
    {
        DuelEvents.Instance.onUpdateUI += UpdateCardInfo;
        raycaster = DuelManager.Instance.GetComponent<GraphicRaycaster>();
    }

    // Updates the card's text fields with data from card
    public abstract void SetCardInfo();
    public abstract void UpdateCardInfo();

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(inHand) {
            hoveredCard = this;
            hoveredCardIndex = transform.GetSiblingIndex();
            hoveredCard.transform.SetAsLastSibling();
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(inHand) {
            if(this == hoveredCard) {
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
}
