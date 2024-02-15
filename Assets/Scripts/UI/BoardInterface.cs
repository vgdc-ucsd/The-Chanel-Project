using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Creates and manages UI components related to the board
public class BoardInterface : MonoBehaviour
{
    // The size of each tile on the canvas
    public Vector2 CellSize = new Vector2Int(100, 100);
    // The size of the grid on the canvas
    public Vector2 GridSize;

    // Holds all board tile GameObjects
    public TileInteractable[,] Tiles;

    public void CreateBoard() {
        DuelSettings settings = DuelManager.Instance.Settings;
        TileInteractable templateTile = DuelManager.Instance.UI.TemplateTile;

        int rows = settings.BoardRows;
        int cols = settings.BoardCols;

        // Destroy all old tiles
        if(Tiles != null) {
            foreach(TileInteractable t in Tiles) {
                Destroy(t.gameObject);
            }
        }

        Tiles = new TileInteractable[rows, cols];

        // Initialze new tiles
        for(int r = 0; r < rows; r++) {
            for(int c = 0; c < cols; c++) { //idk what black magic is going on here but it wouldn't let me use x,y coords
                BoardCoords pos = BoardCoords.FromRowCol(r,c);
                GameObject tile = Instantiate(templateTile.gameObject);
                TileInteractable tileInteractable = tile.GetComponent<TileInteractable>();
                tileInteractable.locationRC = new Vector2Int(r,c);
                tileInteractable.location = pos;
                tile.transform.SetParent(this.transform);
                tile.transform.localScale = Vector3.one;
                tile.SetActive(true);
                Tiles[r,c] = tileInteractable;
            }
        }

        // Set sizes
        GridSize = new Vector2(CellSize.x * cols, CellSize.y * rows);
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = GridSize;
    }

    public void CheckProperInitialization() {
        if(GetComponent<GridLayoutGroup>() == null) {
            Debug.LogWarning("No GridLayoutGroup found on BoardContainer, creating new GridLayoutGroup");
            gameObject.AddComponent<GridLayoutGroup>();
        }
        if(GetComponent<RectTransform>() == null) {
            Debug.LogWarning("No RectTransform found on BoardContainer, creating new RectTransform");
            gameObject.AddComponent<RectTransform>();
        }
    }
}
