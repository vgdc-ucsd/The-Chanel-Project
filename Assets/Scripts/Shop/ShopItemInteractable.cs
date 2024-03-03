using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// The MonoBehavior counterpart for a Card, this is what the user actually interacts with
public class ShopItemInteractable : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    // Determines what card this object is
    public ShopItemData item;

    // Text fields on the card
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;
    public TextMeshProUGUI CardCost;

    // How much the card scales on hover
    private float scaleFactor = 1.1f;

    // Shop Manager Instance
    private ShopManager shopManager;

    // Purchase Manager Instance
    private PurchaseManager purchaseManager;

    private void Awake()
    {
        shopManager = FindObjectOfType<ShopManager>();
        purchaseManager = FindObjectOfType<PurchaseManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // TO BE IMPLEMENTED
            purchaseManager.CardSelected(item);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // FOR TESTING PURPOSES
            shopManager.RemoveItem(item);
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
