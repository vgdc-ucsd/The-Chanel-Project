using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UIManager : MonoBehaviour
{
    // "Blank" objects that are intantiated many times
    public TileInteractable TemplateTile;
    //public CardInteractable TemplateCard;
    public UnitCardInteractable TemplateUnitCard;

    // Interface GameObjects
    public BoardInterface BoardContainer;
    public HandInterface Hand;
    public HandInterface EnemyHand;

    // Player UIs
    public PlayerUI Player;
    public PlayerUI Enemy;

    public void SetupBoard() {
        BoardContainer.CreateBoard();
    }

    public void SetupStatus() {
        Player.Status = DuelManager.Instance.PlayerStatus;
        Enemy.Status = DuelManager.Instance.EnemyStatus;
    }

    public CardInteractable GenerateCardInteractable(Card c) {
        if(c.GetType() == typeof(UnitCard)) {
            UnitCardInteractable ci = Instantiate(TemplateUnitCard);
            ci.card = (UnitCard) c;
            ci.SetCardInfo();
            return ci;
        }
        if(c.GetType() == typeof(SpellCard)) {
            throw new NotImplementedException();
        }
        else {
            Debug.LogError("Unidentified card type");
            return null;
        }
    }

    public TileInteractable FindTileInteractable(BoardCoords bc) {
        if(bc.ToRowColV2().x >= 0 && bc.ToRowColV2().x < BoardContainer.Tiles.GetLength(0)
        && bc.ToRowColV2().y >= 0 && bc.ToRowColV2().y < BoardContainer.Tiles.GetLength(1)) {
            TileInteractable tile = BoardContainer.Tiles[bc.ToRowColV2().x, bc.ToRowColV2().y];
            return tile;
        }
        else {
            Debug.LogWarning("No tile exists at x=" + bc.x + ", y=" + bc.y);
            return null;
        }
    }

    public void CheckProperInitialization() {
        if(TemplateTile == null) {
            Debug.LogError("Cannot create board, TemplateTile is uninitialized");
            return;
        }
        if(TemplateUnitCard == null) {
            Debug.LogError("Could not create hand, TemplateCard is uninitialized");
            return;
        }
        TemplateUnitCard.CheckProperInitialization();

        if(BoardContainer == null) {
            Debug.LogError("Cannot create board, BoardContainer is uninitialized");
            return;
        }
        BoardContainer.CheckProperInitialization();

        if(Hand == null) {
            Debug.LogError("Could not create hand, Hand is uninitialized");
            return;
        }

        if (EnemyHand == null) {
            Debug.LogError("Could not create hand, EnemyHand is uninitialized");
            return;
        }
    }
}
