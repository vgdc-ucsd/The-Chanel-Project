using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UnitCardInteractable : CardInteractable,
    IEndDragHandler,
    IPointerDownHandler
{
    public UnitCard card;

    public GameObject TemplateArrowPlayer;
    public GameObject TemplateArrowEnemy;

    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;

    public override void SetCardInfo() {
        if(card == null) {
            Debug.Log("Could not set card info, card is uninitialzied");
            return;
        }
        CardName.text = card.Name;

        card.UnitCardInteractableRef = this;
        UpdateCardInfo();
    }

    public override void UpdateCardInfo() {
        CardAttack.text = "Attack: " + card.BaseDamage;
        CardHealth.text = "Health: " + card.Health;
        if (inHand) CardCost.text = "Mana Cost: " + card.ManaCost;
    }

    public void DrawArrows() {
        foreach(Attack atk in card.Attacks) {
            GameObject arrow;
            if (card.CurrentTeam == Team.Player) arrow = Instantiate(TemplateArrowPlayer);
            else arrow = Instantiate(TemplateArrowEnemy);

            arrow.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, atk.direction));
            arrow.transform.SetParent(this.transform);
            arrow.transform.localPosition = Vector3.zero;
            arrow.transform.localScale = new Vector3(
                arrow.transform.localScale.x,
                atk.direction.magnitude / 2,
                1
            );
            arrow.SetActive(true);
        }
    }

    // Updates UI to show card being played
    public void PlaceCard(BoardCoords pos)
    {
        TileInteractable tile = BoardInterface.Instance.GetTile(pos);
        if (tile != null) {
            // TODO: move some actions here to PlaceCard in Board
            inHand = false;
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
        TileInteractable tile = BoardInterface.Instance.GetTile(card.Pos);
        transform.position = tile.transform.position;
        transform.SetParent(tile.transform);
        DrawArrows();
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
                if (!DuelManager.Instance.Settings.RestrictPlacement) DuelManager.Instance.TryPlaceCard(card, tile.location);
                else if (tile.location.y <= 1)
                { // can't place in the row closest to enemy
                    DuelManager.Instance.TryPlaceCard(card, tile.location);
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
        if (!inHand) 
        {
            PlayerInputController.Instance.InteractCard(card);
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
}
