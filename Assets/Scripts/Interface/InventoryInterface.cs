// CODE inspired by Ethan, heavily modified by DrFreshPotato

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Creates and manages UI components related to the board
public class InventoryInterface : MonoBehaviour
{
    /* ATM not needed, but may be needed
    
    // The size of each tile on the canvas
    public Vector2 CellSize = new Vector2Int(100, 100);

    // The size of the grid on the canvas
    [SerializeField]
    private Vector2 GridSize;

    */

    //GridLayoutGroup reference, set all Grid settings if needed
    public GridLayoutGroup Grid;

    //Tracks current list of Cards added to Grid
    public GameObject[,] Cards;

    //Uses TemplateCard prefab to host CardInspect script and instantiate GameObjects
    public GameObject prefab;

    public void CreateInventory(Inventory inventory, Deck cardList)
    {
        // Referencing Ethan's code, not sure if need to delete since honestly keeping the list is fine no?
        /*if(Cards != null)
        {
            foreach(GameObject c in Cards)
            {
                Destroy(c);
            }
        }*/

        Cards = new GameObject[inventory.Rows, inventory.Cols];

        int index = 0;
        // Initialize every card in cardList
        for (int i = 0; i < inventory.Rows; i++)
        {
            for (int j = 0; j < inventory.Cols; j++)
            {
                //Checks to see if all cards have been added
                if(index >= cardList.CardList.Count)
                {
                    break;
                }

                prefab.GetComponent<CardInspect>().card = cardList.CardList[index];

                GameObject card = Instantiate(prefab);
                card.transform.SetParent(this.transform);
                card.transform.localScale = Vector3.one;
                card.SetActive(true);
                Cards[i, j] = card;

                index++;
            }
        }

        // Set sizes, referencing Ethan's code 
        /*GridSize = new Vector2(CellSize.x * inventory.Cols, CellSize.y * inventory.Rows);
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = GridSize;*/

        //TODO: Make the RectTransform of this object dynamically change Height based on deck size
    }
}

