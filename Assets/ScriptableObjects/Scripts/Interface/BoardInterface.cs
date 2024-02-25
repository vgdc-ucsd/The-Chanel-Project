using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates and manages UI components related to the board
public class BoardInterface : MonoBehaviour
{
    // The size of each tile on the canvas
    public Vector2 CellSize = new Vector2Int(100, 100);
    // The size of the grid on the canvas
    public Vector2 GridSize;

    // Holds all board tile GameObjects
    public GameObject[,] Tiles;

    public void CreateBoard(Board board, GameObject templateTile) {
        // Destroy all old tiles
        if(Tiles != null) {
            foreach(GameObject t in Tiles) {
                Destroy(t);
            }
        }

        Tiles = new GameObject[board.Rows, board.Cols];

        // Initialze new tiles
        for(int i = 0; i < board.Rows; i++) {
            for(int j = 0; j < board.Cols; j++) {
                GameObject tile = Instantiate(templateTile);
                tile.GetComponent<TileInteractable>().location = new Vector2Int(i, j);
                tile.transform.SetParent(this.transform);
                tile.transform.localScale = Vector3.one;
                tile.SetActive(true);
                Tiles[i, j] = tile;
            }
        }

        // Set sizes
        GridSize = new Vector2(CellSize.x * board.Cols, CellSize.y * board.Rows);
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = GridSize;
    }
}
