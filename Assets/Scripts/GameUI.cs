using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // General UI not related to either player
    [SerializeField] TMP_Text turnNumberText;
    [SerializeField] TMP_Text teamText;

    void UpdateGameTurn()
    {
        // TODO
        // turnNumberText.text = DuelManger.Instance.turnNumber.ToString();
    }

    void UpdateTeam()
    {
        // TODO
        // teamText.text = = DuelManger.Instance.currentTeam.ToString();
    }
}
