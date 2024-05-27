
// Handles player input of actions on the board, e.g. move card, activate abilities, etc
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

enum ControlAction
{
    None, Move
}

public class PlayerInputController: MonoBehaviour
{
    public UnitCard selectedCard;
    ControlAction currentAction;
    public static PlayerInputController Instance;

    public void Awake()
    {
        Instance = this; 
    }

    // Determines the pending action, i.e. what the next player interaction of a card/tile will do
    private void SetAction(ControlAction action)
    {
        switch (action)
        {
            case ControlAction.None:
                foreach (TileInteractable tile in BoardInterface.Instance.Tiles)
                {
                    tile.SetHighlight(false);
                }
                currentAction = action;
                return;
            case ControlAction.Move:
                foreach (BoardCoords adj in DuelManager.Instance.MainDuel.DuelBoard.GetEmptyAdjacentTiles(selectedCard.Pos))
                {
                    BoardInterface.Instance.GetTile(adj).SetHighlight(true);
                }
                currentAction = action;
                return;
        }
    }
    
    // Handle any input that involves clicking a card on the board
    public void InteractCard(UnitCard card)
    {
        if (!card.UnitCardInteractableRef.CanInteract || (card.CurrentTeam == Team.Enemy && !DuelManager.Instance.Settings.EnablePVPMode)) return;
        SetAction(ControlAction.None);
        //if (card.team != DuelManager.Instance.DC.GetCurrentTeam())
        //{
        //    ClearSelection();
        //    return;
        //} TODO

        // for now, unselect a card by clicking it again
        // will have better control later
        if (card == selectedCard)
        {
            ClearSelection();
            return;
        }
        SelectCard(card, true);
        
        if(card.CanMove) {
            SetAction(ControlAction.Move);
        }
    }

    // Toggles the selection state of a card and updates the previously selected card
    public void SelectCard(UnitCard card, bool toggle = true)
    {
        if (card == null) return;
        if (card.CurrentTeam == Team.Enemy && !DuelManager.Instance.Settings.EnablePVPMode) return;
        if (toggle && selectedCard != null && selectedCard != card)
        {
            // unselect previous card
            selectedCard.SetSelected(false);
        }
        
        if (toggle)
        {
            card.SetSelected(true);
            selectedCard = card;
        }
        else
        {
            card.SetSelected(false);
            selectedCard = null;
        }
    }

    public void ClearSelection()
    {
        if (selectedCard != null) SelectCard(selectedCard, false);
        SetAction(ControlAction.None);

    }

    // Handle any input that involves clicking a tile
    public void InteractTile(BoardCoords pos)
    {
        if (currentAction == ControlAction.Move)
        {
            // TODO check that it is the player's turn
            if (DuelManager.Instance.MainDuel.DuelBoard.IsOccupied(pos)) return;
            if (!DuelManager.Instance.MainDuel.DuelBoard.GetEmptyAdjacentTiles(selectedCard.Pos).Contains(pos)) return;
            TileInteractable tile = BoardInterface.Instance.GetTile(pos);

            DuelManager.Instance.MainDuel.DuelBoard.MoveCard(selectedCard, pos, DuelManager.Instance.MainDuel);

            // animation
            float speed = 0.5f;
            Transform targetTransform = BoardInterface.Instance.GetTile(pos).transform;
            IEnumerator ie = AnimationManager.Instance.MoveCard(selectedCard, targetTransform, speed);
            AnimationManager.Instance.Play(ie);

            SelectCard(selectedCard, false);
            selectedCard = null;
            SetAction(ControlAction.None);

            // SFX
            FMODUnity.RuntimeManager.PlayOneShot("event:/CardMove", transform.position);
        }
    }
}