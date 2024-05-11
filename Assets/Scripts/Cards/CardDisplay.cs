using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Strictly for Displaying Card Details onto Card
 * Without having to deal with combat functions
 */


public class CardDisplay : MonoBehaviour
{
    // Card to Display
    [SerializeField]
    private Card card;

    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;
    public TextMeshProUGUI CardMana;
    public TextMeshProUGUI CardCost;

    public Image CardArt;

    public void setDisplay(UnitCard card)
    {
        this.card = card;

        CardName.text = card.Name;
        CardHealth.text = card.Health.ToString();
        CardAttack.text = card.BaseDamage.ToString();
        CardMana.text = card.ManaCost.ToString();
        CardCost.text = card.ShopCost.ToString() + "g";

        CardArt.sprite = card.Artwork;
    }
}
