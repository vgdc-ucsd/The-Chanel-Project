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

    private List<GameObject> arrows = new List<GameObject>();

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
        foreach (GameObject obj in arrows)
        {
            Destroy(obj);
        }
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
            arrows.Add(arrow);
        }
    }

    // Updates UI to show card being played
    public void UIPlaceCard(BoardCoords pos)
    {
        TileInteractable tile = BoardInterface.Instance.GetTile(pos);
        if (tile != null) {
            // TODO: move some actions here to PlaceCard in Board
            inHand = false;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.position = tile.transform.position;
            if(handInterface != null) {
                handInterface.cardObjects.Remove(this.gameObject);
            } 
            transform.SetParent(tile.transform);
            transform.localScale = Vector3.one;
            DrawArrows(); 
            CardCost.enabled = false;
            gameObject.SetActive(true);
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

    // attempts to play card at specified position, and calls Board to play card
    // if successful
    public override void TryPlayCard(BoardCoords pos)
    {
        if (!DuelManager.Instance.Settings.RestrictPlacement || pos.y <= 1)
        {
            // Check out of bounds
            if (DuelManager.Instance.MainDuel.DuelBoard.IsOutOfBounds(pos)) return;
            if (DuelManager.Instance.MainDuel.DuelBoard.IsOccupied(pos)) return;

            // TODO
            //if (currentTeam != card.team) {
            //    Debug.Log($"Tried to play {card.team} card while on {currentTeam} turn");
            //    return;
            //}
            CharStatus charStatus;
            if (card.CurrentTeam == Team.Player) charStatus = DuelManager.Instance.MainDuel.PlayerStatus;
            else charStatus = DuelManager.Instance.MainDuel.EnemyStatus;

            if (!charStatus.CanUseMana(card.ManaCost))
            {
                Debug.Log("Not enough Mana"); //TODO: UI feedback
                return;
            }
            //if(card.team == Team.Enemy) MirrorAttacks(card); // this should only be called once per enemy card

            DuelManager.Instance.MainDuel.DuelBoard.PlayCard(card, pos, charStatus, DuelManager.Instance.MainDuel);
            IEnumerator ie = AnimationManager.Instance.PlaceUnitCard(card, pos, 0.0f);
            AnimationManager.Instance.Play(ie);
            UIManager.Instance.UpdateStatus(DuelManager.Instance.MainDuel);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inHand) 
        {
            PlayerInputController.Instance.InteractCard(card);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        UIManager.Instance.InfoPanel.UpdateInfoPanelUnitCard(this.card);
        AnimationManager.Instance.StartManaHover(card.ManaCost, card.CurrentTeam);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        AnimationManager.Instance.StopManaHover(card.CurrentTeam);
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
