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
    // Sets the size of the board
    public int InventoryRows;
    public int InventoryCols;

    // "Blank" objects that are intantiated many times
    public GameObject TemplateTile;
    public GameObject TemplateCard;

    // Interface GameObjects
    public GameObject InventoryContainer;


    // Stores information on the current game state
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory(InventoryRows, InventoryCols);
        InitializeInventory();
    }

    // Create new Inventory
    private void InitializeInventory()
    {
        InventoryContainer.GetComponent<InventoryInterface>().CreateInventory(inventory, TemplateTile);
    }

    private void CheckProperInitialization()
    {
        if (inventory == null)
        {
            Debug.Log("Cannot create board, board is uninitialized");
            return;
        }
        if (TemplateTile == null)
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
        if (TemplateTile.GetComponent<TileInteractable>() == null)
        {
            Debug.Log("No TileInteractable found on TemplateTile, creating new TileInteractable");
            TemplateTile.AddComponent<TileInteractable>();
        }
    }
}
