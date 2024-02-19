using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // General UI not related to either player
    [SerializeField] TMP_Text turnNumberText;
    private void Awake()
    {
        DuelEvents.Instance.OnAdvanceGameTurn += UpdateGameTurn;
    }

    void UpdateGameTurn()
    {
        turnNumberText.text = DuelManager.Instance.DC.turnNumber.ToString();
    }
}
