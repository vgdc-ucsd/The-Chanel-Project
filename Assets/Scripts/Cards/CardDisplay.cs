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
    public Card card;

    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;
    public TextMeshProUGUI CardMana;
    public TextMeshProUGUI CardCost;

    public Texture2D CardArt;

    void Start()
    {
        CardName.text = card.Name;
        CardHealth.text = "Health: " + card.Health;
        CardAttack.text = "Attack: " + card.AttackDamage;
        CardMana.text = card.ManaCost.ToString();
        CardCost.text = card.ShopCost.ToString();

        CardArt = card.Artwork;
    }



}
