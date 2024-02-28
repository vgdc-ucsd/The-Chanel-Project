using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * IMPORTANT: Place Script in ShopUI GameObject
 * 
 */
public class ShopInteractable : MonoBehaviour
{
    // Button reference of Purchaseable Item
    private Button itemClick;

    // Reference to shopManager script
    private ShopManager shopManager;

    // Prefab ShopCardSlot
    public GameObject ShopCardSlotPrefab;

    // Reference to Parent GameObject with GridLayoutGroup
    [SerializeField]
    private Transform parentCards;

    void Start()
    {
        // Automatically Finds and Assigns ShopManager script
        shopManager = gameObject.GetComponent<ShopManager>();
        if (shopManager == null)
        {
            Debug.LogError("Failed to find ShopManager script in current GameObject");
        }

        // Automatically Finds and Assigns Cards(GridLayOut) GameObject, found by index!!!
        parentCards = gameObject.transform.GetChild(0).transform.GetChild(0);
        if (parentCards == null)
        {
            Debug.LogError("Failed to find Parent GameObject");
        }

        setupCards();   
    }

    private void setupCards()
    {
        //Loops for the entire size of the Shop, instantiating the cards
        for(int i = 0; i < shopManager.rowSize * shopManager.colSize; i++)
        {
            GameObject cardPrefab = Instantiate(ShopCardSlotPrefab, parentCards);
        }
    }
}
