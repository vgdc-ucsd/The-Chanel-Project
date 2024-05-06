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
    public SpellCardInteractable TemplateSpellCard;

    // Interface GameObjects
    public BoardInterface BoardContainer;
    public HandInterface Hand;
    public HandInterface EnemyHand;

    // Player UIs
    public PlayerUI Player;
    public PlayerUI Enemy;

    public void Initialize() {
        BoardContainer.CreateBoard();
    }

    public void UpdateStatus(DuelInstance state) {
        Player.UpdateUI(state.PlayerStatus);
        Enemy.UpdateUI(state.EnemyStatus);
    }

    public CardInteractable GenerateCardInteractable(Card c) {
        if(c is UnitCard) {
            UnitCardInteractable ci = Instantiate(TemplateUnitCard);
            ci.card = (UnitCard) c;
            ci.SetCardInfo();

            if(c.CurrentTeam == Team.Player) ci.handInterface = Hand;
            else ci.handInterface = EnemyHand;

            ci.handInterface.cardObjects.Add(ci);
            return ci;
        }
        else if(c is SpellCard) {
            SpellCardInteractable ci = Instantiate(TemplateSpellCard);
            ci.card = (SpellCard)c;
            ci.SetCardInfo();

            if (c.CurrentTeam == Team.Player) ci.handInterface = Hand;
            else ci.handInterface = EnemyHand;

            ci.handInterface.cardObjects.Add(ci);
            return ci;
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
