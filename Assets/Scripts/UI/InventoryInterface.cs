// CODE ENTIRELY RIPPED FROM ETHAN S COMMITS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates and manages UI components related to the board
public class InventoryInterface : MonoBehaviour
{
    // The size of each tile on the canvas
    public Vector2 CellSize = new Vector2Int(100, 100);
    // The size of the grid on the canvas
    public Vector2 GridSize;

    // Holds all board tile GameObjects
    public GameObject[,] Tiles;

    public void CreateInventory(Inventory inventory, GameObject templateTile)
    {
        // Destroy all old tiles
        if (Tiles != null)
        {
            foreach (GameObject t in Tiles)
            {
                Destroy(t);
            }
        }

        Tiles = new GameObject[inventory.Rows, inventory.Cols];

        // Initialze new tiles
        for (int i = 0; i < inventory.Rows; i++)
        {
            for (int j = 0; j < inventory.Cols; j++)
            {
                GameObject tile = Instantiate(templateTile);
                tile.GetComponent<TileInteractable>().locationRC = new Vector2Int(i, j);
                tile.transform.SetParent(this.transform);
                tile.transform.localScale = Vector3.one;
                tile.SetActive(true);
                Tiles[i, j] = tile;
            }
        }

        // Set sizes
        GridSize = new Vector2(CellSize.x * inventory.Cols, CellSize.y * inventory.Rows);
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = GridSize;
    }
}

