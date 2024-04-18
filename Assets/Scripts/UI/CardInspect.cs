using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInspect : MonoBehaviour
{
    public UnitCard card;

    // Text fields on the card
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;

    // How much the card scales on hover
    public float scaleFactor = 1f;

    // Updates the card's text fields with data from card
    public void SetCardInfo()
    {
        if (card == null)
        {
            Debug.Log("Could not set card info, card is uninitialzied");
            return;
        }
        CardName.text = card.Name;
        CardAttack.text = "Attack: " + card.BaseDamage; 
        CardHealth.text = "Health: " + card.Health;
    }
}
