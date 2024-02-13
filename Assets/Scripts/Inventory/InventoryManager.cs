// CODE ENTIRELY RIPPED FROM ETHAN S COMMITS

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// The main manager for duels/combat, handles all things related to duels
public class InventoryManager : MonoBehaviour
{
    // Sets the size of the board, *rows are infinite/based on deck size*
    private int InventoryRows;
    public int InventoryCols;

    // Stores the card slots 
    public GameObject cardSlots;

    //PlayerDeck ScriptableObject 
    public Deck PlayerDeck;

    // Interface GameObjects
    public GameObject InventoryContainer;


    // Reference to BaseInventory ScriptableObject
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        // round up the division to find # of rows needed
        InventoryRows = (PlayerDeck.CardList.Count + InventoryCols - 1) / InventoryCols;

        inventory = new Inventory(InventoryRows, InventoryCols);
        InitializeInventory();
        CheckProperInitialization();
    }

    // Create new Inventory
    private void InitializeInventory()
    {
        InventoryContainer.GetComponent<InventoryInterface>().CreateInventory(inventory, PlayerDeck);
        //InventoryContainer.GetComponent<InventoryInterface>().CreateInventory(inventory, cardSlots);
    }

    private void CheckProperInitialization()
    {
        if (inventory == null)
        {
            Debug.Log("Cannot create board, board is uninitialized");
            return;
        }
        if (cardSlots == null)
        {
            Debug.Log("Cannot create board, TemplateTile is uninitialized");
            return;
        }
        if (InventoryContainer == null)
        {
            Debug.Log("Cannot create board, BoardContainer is uninitialized");
            return;
        }

        if (InventoryContainer.GetComponent<BoardInterface>() == null)
        {
            Debug.Log("No BoardInterface found on BoardContainer, creating new BoardInterface");
            InventoryContainer.AddComponent<BoardInterface>();
        }
        if (InventoryContainer.GetComponent<GridLayoutGroup>() == null)
        {
            Debug.Log("No GridLayoutGroup found on BoardContainer, creating new GridLayoutGroup");
            InventoryContainer.AddComponent<GridLayoutGroup>();
        }
        if (InventoryContainer.GetComponent<RectTransform>() == null)
        {
            Debug.Log("No RectTransform found on BoardContainer, creating new RectTransform");
            InventoryContainer.AddComponent<RectTransform>();
        }
        if (cardSlots.GetComponent<TileInteractable>() == null)
        {
            Debug.Log("No TileInteractable found on TemplateTile, creating new TileInteractable");
            cardSlots.AddComponent<TileInteractable>();
        }
    }
}
