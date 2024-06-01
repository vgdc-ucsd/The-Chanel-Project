using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class UnitCardInteractable : CardInteractable,
    IEndDragHandler,
    IPointerDownHandler
{
    public UnitCard card;

    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;
    public Image CardArt;
    public Image Glow;

    public List<Image> Arrows = new List<Image>();
    public Sprite InactiveArrowOrthogonal;
    public Sprite ActiveArrowOrthogonalPlayer;
    public Sprite ActiveArrowOrthogonalEnemy;
    public Sprite InactiveArrowDiagonal;
    public Sprite ActiveArrowDiagonalPlayer;
    public Sprite ActiveArrowDiagonalEnemy;

    public StatusIconManager icons;

    private Vector2Int UpLeft = new Vector2Int(-1, 1);
    private Vector2Int UpMid = new Vector2Int(0, 1);
    private Vector2Int UpRight = new Vector2Int(1, 1);
    private Vector2Int Left = new Vector2Int(-1, 0);
    private Vector2Int Right = new Vector2Int(1, 0);
    private Vector2Int DownLeft = new Vector2Int(-1, -1);
    private Vector2Int DownMid = new Vector2Int(0, -1);
    private Vector2Int DownRight = new Vector2Int(1, -1);

    //private FMODUnity.StudioEventEmitter emitter;
    //private string eventPath = "";

    private Vector3 scaleVector = new Vector3(0.8f, 0.8f, 1.0f);

    protected override void Awake()
    {
        base.Awake();
        if (icons != null) icons.ci = this;

        // Audio
        //emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    public override void SetCardInfo()
    {
        if (card == null)
        {
            Debug.Log("Could not set card info, card is uninitialzied");
            return;
        }
        CardName.text = card.Name;

        card.UnitCardInteractableRef = this;
        UpdateCardInfo();
    }

    public override void UpdateCardInfo()
    {
        CardAttack.text = card.BaseDamage.ToString();
        CardHealth.text = card.Health.ToString();
        if (CardArt != null)
        {
            CardArt.sprite = card.Artwork;
        }
        if (inHand) CardCost.text = card.ManaCost.ToString();
        icons.RefreshIcons();
    }

    public void UpdateCardInfoDamage(int damage)
    {
        CardHealth.text = Math.Max(int.Parse(CardHealth.text) - damage, 0).ToString();
    }

    public void UpdateCardAttack(int amount = 0)
    {
        int currentAttack = int.Parse(CardAttack.text);
        CardAttack.text = (currentAttack + amount).ToString();
    }

    public void AddStatusEffectIcon(StatusEffect statusEffect, int duration)
    {
        icons.AddIcon(statusEffect, duration);
    }

    public void UpdateStatusEffectIcon(StatusEffect statusEffect, int amount)
    {
        icons.UpdateIcon(statusEffect, amount);
    }

    public void DrawArrows()
    {
        ResetArrows();
        Mana.gameObject.SetActive(false);

        Sprite ActiveArrowDiagonal;
        Sprite ActiveArrowOrthogonal;
        if (card.CurrentTeam == Team.Enemy)
        {
            ActiveArrowDiagonal = ActiveArrowDiagonalEnemy;
            ActiveArrowOrthogonal = ActiveArrowOrthogonalEnemy;
        }
        else
        {
            ActiveArrowDiagonal = ActiveArrowDiagonalPlayer;
            ActiveArrowOrthogonal = ActiveArrowOrthogonalPlayer;
        }

        foreach (Attack atk in card.Attacks)
        {
            Vector2Int dir = atk.direction;
            if (dir == UpLeft) Arrows[0].sprite = ActiveArrowDiagonal;
            else if (dir == UpMid) Arrows[1].sprite = ActiveArrowOrthogonal;
            else if (dir == UpRight) Arrows[2].sprite = ActiveArrowDiagonal;
            else if (dir == Left) Arrows[3].sprite = ActiveArrowOrthogonal;
            else if (dir == Right) Arrows[4].sprite = ActiveArrowOrthogonal;
            else if (dir == DownLeft) Arrows[5].sprite = ActiveArrowDiagonal;
            else if (dir == DownMid) Arrows[6].sprite = ActiveArrowOrthogonal;
            else if (dir == DownRight) Arrows[7].sprite = ActiveArrowDiagonal;
            else
            {
                Debug.LogWarning($"Could not draw arrows for attack direction {dir}");
            }
        }
    }

    public void ResetArrows()
    {
        Mana.gameObject.SetActive(true);
        for (int i = 0; i < 8; i++)
        {
            if (i == 1 || i == 3 || i == 4 || i == 6)
            {
                Arrows[i].sprite = InactiveArrowOrthogonal;
            }
            else
            {
                Arrows[i].sprite = InactiveArrowDiagonal;
            }
        }
    }

    // Updates UI to show card being played
    public void UIPlaceCard(BoardCoords pos)
    {
        TileInteractable tile = BoardInterface.Instance.GetTile(pos);
        if (tile != null)
        {
            // TODO: move some actions here to PlaceCard in Board
            inHand = false;
            transform.localEulerAngles = Vector3.zero;
            transform.position = tile.transform.position;
            if (handInterface != null)
            {
                handInterface.cardObjects.Remove(this.gameObject);
            }
            transform.SetParent(tile.transform);
            transform.localScale = scaleVector;
            DrawArrows();
            CardCost.enabled = false;
            gameObject.SetActive(true);
            //handInterface.OrganizeCards();
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/CardPlace", transform.position);
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
            if (DuelManager.Instance.MainDuel.DuelBoard.IsOutOfBounds(pos))
            {
                ResetArrows();
                return;
            }
            if (DuelManager.Instance.MainDuel.DuelBoard.IsOccupied(pos))
            {
                ResetArrows();
                return;
            }

            // TODO
            //if (currentTeam != card.team) {
            //    Debug.Log($"Tried to play {card.team} card while on {currentTeam} turn");
            //    return;
            //}
            CharStatus charStatus;
            if (card.CurrentTeam == Team.Player)
            {
                charStatus = DuelManager.Instance.MainDuel.PlayerStatus;
                UIManager.Instance.Player.UnhoverMana(charStatus);
            }
            else
            {
                charStatus = DuelManager.Instance.MainDuel.EnemyStatus;
                UIManager.Instance.Enemy.UnhoverMana(charStatus);
            }

            if (!charStatus.CanUseMana(card.ManaCost))
            {
                Debug.Log("Not enough Mana"); //TODO: UI feedback
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/NoMana"); // SFX

                return;
            }
            //if(card.team == Team.Enemy) MirrorAttacks(card); // this should only be called once per enemy card
            DuelManager.Instance.MainDuel.DuelBoard.PlayCard(card, pos, charStatus, DuelManager.Instance.MainDuel);
            IEnumerator ie = AnimationManager.Instance.PlaceUnitCard((UnitCard)card.Clone(), pos, 0.0f);
            AnimationManager.Instance.Play(ie);
            UIManager.Instance.UpdateStatus(DuelManager.Instance.MainDuel);
        }


    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (inHand && CanInteract && mode == CIMode.Duel)
        {
            DrawArrows();
        }
        base.OnBeginDrag(eventData);

        if (mode == CIMode.Inventory) return;
        // FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/CardSlide", transform.position); // Only want for when clicked/moving from deck
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (inHand && CanInteract && mode == CIMode.Duel)
        {
            ResetArrows();
        }
        base.OnEndDrag(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (mode == CIMode.Inventory)
        {
            InventoryUI.Instance.HandleClick(this);
        }
        else if (mode == CIMode.Duel)
        {
            if (!inHand)
            {
                PlayerInputController.Instance.InteractCard(card);
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (mode == CIMode.Display) return;
        // Inventory stuff
        if (mode == CIMode.Inventory)
        {
            InventoryUI.Instance.inventoryInfoPanel.UpdateInventoryInfoPanelUnitCard(this.card);
        }

        if (mode != CIMode.Duel) return;
        if (UIManager.Instance.InfoPanel != null) { UIManager.Instance.InfoPanel.UpdateInfoPanelUnitCard(this.card); }
        if (!CanInteract || !inHand) return;
        AnimationManager.Instance.StartManaHover(card.ManaCost, card.CurrentTeam);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (mode == CIMode.Display) return;
        if (mode != CIMode.Duel) return;
        AnimationManager.Instance.StopManaHover(card.CurrentTeam);
    }

    public override Card GetCard()
    {
        return card;
    }
}
