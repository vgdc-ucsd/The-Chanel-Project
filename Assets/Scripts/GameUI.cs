using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // General UI not related to either player
    [SerializeField] TMP_Text turnNumberText;
    [SerializeField] TMP_Text teamText;
    private void Awake()
    {
        DuelEvents.Instance.OnAdvanceGameTurn += UpdateGameTurn;
        DuelEvents.Instance.onTurnStart += UpdateTeam;
    }

    void UpdateGameTurn()
    {
        turnNumberText.text = DuelManager.Instance.DC.turnNumber.ToString();
    }

    void UpdateTeam()
    {
        teamText.text = DuelManager.Instance.DC.GetCurrentTeam().ToString();
    }
}
