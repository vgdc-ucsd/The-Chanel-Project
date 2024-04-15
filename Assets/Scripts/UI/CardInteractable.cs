using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

// The MonoBehavior counterpart for a Card, this is what the user actually interacts with
public class CardInteractable : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IBeginDragHandler, 
    IEndDragHandler,
    IDragHandler,
    IPointerDownHandler
{
    // Data for the card it contains
    [HideInInspector] public Card card;

    // Reference to the HandInterface
    [HideInInspector] public HandInterface handInterface;

    // fields set through inspector
    public GraphicRaycaster raycaster; 
    public GameObject TemplateArrowPlayer;
    public GameObject TemplateArrowEnemy;
    public Canvas canvas;

    // Text fields on the card
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;
    public TextMeshProUGUI CardCost;
    public TextMeshProUGUI DebugText;

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

    public void DrawArrows() {
        if (card is UnitCard unit)
        {
            foreach (Vector2Int v in AttackDirections.AllAttackDirections)
            {
                if (unit.GetAttack(v) != null)
                {
                    GameObject arrow;
                    if (unit.team == Team.Player)
                    {
                        arrow = Instantiate(TemplateArrowPlayer);
                    }
                    else
                    {
                        arrow = Instantiate(TemplateArrowEnemy);
                    }

                    arrow.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, v));
                    arrow.transform.SetParent(this.transform);
                    arrow.transform.localPosition = Vector3.zero;
                    arrow.transform.localScale = new Vector3(
                        arrow.transform.localScale.x,
                        v.magnitude / 2,
                        1
                    );
                    arrow.SetActive(true);
                }
            }
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
        if (card is UnitCard unit)
        {
            CardAttack.text = "Attack: " + unit.AttackDamage;
            CardHealth.text = "Health: " + unit.Health;
            if (inHand) CardCost.text = "Mana Cost: " + unit.ManaCost;
        }
    }

    // Updates UI to show card being played
    public void PlaceCard(BoardCoords pos)
    {
        if (card is SpellCard)
        {
            Destroy(gameObject);
            return;
        }
        TileInteractable tile = BoardInterface.Instance.GetTile(pos);
        if (tile != null) {
            // TODO: move some actions here to PlaceCard in Board
            inHand = false;
            ToggleVisibility(true);
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.position = tile.transform.position;
            if(handInterface != null) {
                handInterface.cardObjects.Remove(this);
            } 
            transform.SetParent(tile.transform);
            transform.localScale = Vector3.one;
            DrawArrows(); 
            CardCost.enabled = false;
            //handInterface.OrganizeCards();
        }
    }

    public void UpdateCardPos()
    {
        if (card is UnitCard unit) { 
            TileInteractable tile = BoardInterface.Instance.GetTile(unit.pos);
            transform.position = tile.transform.position;
            transform.SetParent(tile.transform);
            DrawArrows();
        }   
    }



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

            if (tile != null)
            {
                if (!DuelManager.Instance.Settings.RestrictPlacement) DuelManager.Instance.PlaceCard(card, tile.location);
                else if (tile.location.y <= 1)
                { // can't place in the row closest to enemy
                    DuelManager.Instance.PlaceCard(card, tile.location);
                }
            }
            // Reorganize the player's hand
            if(handInterface == null) {
                Debug.Log("Could not organize hand, handInterface is uninitialized");
                return;
            }
            
        }
        handInterface.OrganizeCards();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inHand && card is UnitCard unit) 
        {
            PlayerInputController.Instance.InteractCard(unit);
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

    public void CheckProperInitialization() {
        if(TemplateArrowPlayer == null) {
            Debug.LogError("Could not create hand, TemplateCard is has no TemplateArrowPlayer");
            return;
        }
        if(TemplateArrowEnemy == null) {
            Debug.LogError("Could not create hand, TemplateCard is has no TemplateArrowEnemy");
            return;
        }
    }

    public void ToggleVisibility(bool toggle)
    {
        canvas.enabled = toggle;
    }
}
