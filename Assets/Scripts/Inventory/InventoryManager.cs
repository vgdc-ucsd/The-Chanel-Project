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
}
