using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
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


    public event Action onTurnEnd;
    public void TurnEnd()
    {
        if (onTurnEnd != null) onTurnEnd();
    }

    public event Action onUpdateUI;
    public void UpdateUI()
    {
        if (onUpdateUI != null) onUpdateUI();
    }

    public void DrawCardPlayer(Card c)
    {
        if (OnDrawCardPlayer != null) OnDrawCardPlayer(c);
        if (OnDrawCardAny != null) OnDrawCardAny(c);

    }
    public event Action<Card> OnDrawCardPlayer;

    public void DrawCardEnemy(Card c)
    {
        if (OnDrawCardEnemy != null) OnDrawCardEnemy(c);
        if (OnDrawCardAny != null) OnDrawCardAny(c);
    }

    public event Action<Card> OnDrawCardEnemy;
    public event Action<Card> OnDrawCardAny;
}