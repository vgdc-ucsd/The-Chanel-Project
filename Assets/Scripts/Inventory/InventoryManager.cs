using UnityEngine;

// The main manager for duels/combat, handles all things related to duels
public class InventoryManager : MonoBehaviour
{
    // Sets the size of the board
    public int InventoryRows;
    public int InventoryCols;

    // Stores information on the current game state
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory(InventoryRows, InventoryCols);
    }
}
