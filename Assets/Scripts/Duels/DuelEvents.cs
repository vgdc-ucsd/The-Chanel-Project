using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UIElements; - Bugging the Build
using UnityEngine;

public class DuelEvents : MonoBehaviour
{
    public static DuelEvents Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public event Action onDebug;
    public void _Debug()
    {
        if (onDebug != null) onDebug();
    }


    public event Action onTurnEnd; // triggers right after the player is no longer able to make moves (after clicking end turn)
    public void TurnEnd()
    {
        if (onTurnEnd != null) onTurnEnd();
    }

    public event Action onTurnStart; // triggers right before the player is about to move
    public void TurnStart()
    {
        if (onTurnStart != null) onTurnStart();
    }

    public event Action onUpdateUI;
    public void UpdateUI()
    {
        if (onUpdateUI != null) onUpdateUI();
    }

    public event Action onUpdateHand;
    public void UpdateHand()
    {
        if (onUpdateHand != null) onUpdateHand();
    }

    public event Action<Card, Team> OnDrawCard;
    public void DrawCard(Card c, Team team)
    {
        if (OnDrawCard != null) OnDrawCard(c, team);
    }

    public event Action<Card, BoardCoords, Team> OnPlaceCard;
    public event Action<Card> OnRemoveFromHand;
    public void PlaceCard(Card c, BoardCoords pos, Team team)
    {
        if (OnPlaceCard != null)
        {
            OnPlaceCard(c, pos, team);
            OnRemoveFromHand(c);
        }
    }

    public event Action OnAdvanceGameTurn;
    public void AdvanceGameTurn()
    {
        if (OnAdvanceGameTurn != null) OnAdvanceGameTurn();
    }


}