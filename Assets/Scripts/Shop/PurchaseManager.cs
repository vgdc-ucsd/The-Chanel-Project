using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseManager : MonoBehaviour
{
    static public int money = 0;
    public TextMeshProUGUI PlayerMoney;

    // Determines what card this object is
    private ShopItemData CardItem;

    // Text fields on the card
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;
    public TextMeshProUGUI CardCost;
    public TextMeshProUGUI CardExcerpt;

    // Shop Manager Instance
    private ShopManager shopManager;

    public GameObject toBuy;

    public Button buyButton;

    private void Awake()
    {
        shopManager = FindObjectOfType<ShopManager>();
    }

    //TODO: DELETE, DEBUG PURPOSES ONLY
    public void GivePlayerMoney(){
        money++;
        UpdateMoneyText();
    }

    public void CardSelected(ShopItemData item){
        toBuy.SetActive(true);
        CardName.text = item.name;
        CardHealth.text = "Health: " + item.health;
        CardAttack.text = "Attack: " + item.attack;
        CardCost.text = "Cost: " + item.cost;
        CardExcerpt.text = "\"" + item.excerpt + "\"";
        CardItem = item;

        //If you don't have enough money, the "buy" button is disabled. Otherwise, it's interactable.
        buyButton.interactable = (item.cost>money)?false:true;     
    }

    public void PurchaseCancelled(){
        toBuy.SetActive(false);
    }

    public void PurchaseConfirmed(){
        money -= CardItem.cost;
        UpdateMoneyText();
        //TODO: Add this card to the inventory.
        toBuy.SetActive(false);
        shopManager.RemoveItem(CardItem);
    }

    private void UpdateMoneyText(){
        PlayerMoney.text = "Money: " + money;
    }
}
