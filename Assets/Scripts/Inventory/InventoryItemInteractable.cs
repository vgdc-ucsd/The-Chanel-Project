using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemInteractable : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    // Determines what card this object is
    public Card card;

    // How much the card scales on hover
    private float scaleFactor = 1.1f;

    // Inventory Manager Instance
    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // TO BE IMPLEMENTED
            Debug.Log("Show more details");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // FOR TESTING PURPOSES
            inventoryManager.RemoveItem(card);
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
