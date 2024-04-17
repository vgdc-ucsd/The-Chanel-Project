using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * IMPORTANT: Place Script in ShopUI GameObject
 * 
 * Controls MonoBehaviour for Cards in Shop
 */
public class ShopCardInteractable : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    // Reference to shopManager script
    private ShopManager shopManager;

    // Collection of Cards Player Unlocked
    public Deck cardCollection;

    // Takes in Card object
    public Card card;

    // Display Card Info
    private CardDisplay display;

    // How much the card scales on hover
    private float scaleFactor = 1.1f;

    //Awake() to be First before Start()
    private void Awake()
    {
        // Assigns Necessary Scripts
        shopManager = FindObjectOfType<ShopManager>();
        display = this.gameObject.GetComponent<CardDisplay>();

        if(shopManager == null || display == null)
        {
            Debug.LogWarning("Missing shopManager or display script");
        }

        // Generates Unique Random Card from Card Collection
        int collectionSize = cardCollection.CardList.Count - 1;
        int randomIndex = shopManager.generateNum(collectionSize);
        Card randomCard = cardCollection.CardList[randomIndex];

        display.setDisplay(randomCard);
        card = randomCard;
    }

    // Left Click to Buy, Right Click to Inspect
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            this.gameObject.SetActive(false);
            shopManager.purchase(card);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            shopManager.inspect(card);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}
