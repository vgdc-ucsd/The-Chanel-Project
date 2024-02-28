using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// The MonoBehavior counterpart for a Card, this is what the user actually interacts with
public class InventoryItemInteractable : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    // Determines what card this object is
    public InventoryItemData item;

    // Text fields on the card
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardHealth;
    public TextMeshProUGUI CardAttack;

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
            inventoryManager.RemoveItem(item);
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
