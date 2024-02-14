using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // "Blank" objects that are intantiated many times
    public TileInteractable TemplateTile;
    public CardInteractable TemplateCard;

    // Interface GameObjects
    public BoardInterface BoardContainer;
    public HandInterface Hand;

    // Player UIs
    public PlayerUI Player;
    public PlayerUI Enemy;

    public void SetupBoard() {
        BoardContainer.CreateBoard();
    }

    public void SetupHand() {
        Hand.TemplateCard = TemplateCard;
    }

    public void CheckProperInitialization() {
        if(TemplateTile == null) {
            Debug.LogError("Cannot create board, TemplateTile is uninitialized");
            return;
        }
        if(TemplateCard == null) {
            Debug.LogError("Could not create hand, TemplateCard is uninitialized");
            return;
        }
        TemplateCard.CheckProperInitialization();

        if(BoardContainer == null) {
            Debug.LogError("Cannot create board, BoardContainer is uninitialized");
            return;
        }
        BoardContainer.CheckProperInitialization();

        if(Hand == null) {
            Debug.LogError("Could not create hand, Hand is uninitialized");
            return;
        }
    }
}
