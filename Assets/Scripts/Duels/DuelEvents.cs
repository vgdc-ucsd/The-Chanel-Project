using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class DuelEvents : MonoBehaviour
{
    public static DuelEvents instance;
    [SerializeField]

    private void Awake()
    {
        instance = this;
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
    
}