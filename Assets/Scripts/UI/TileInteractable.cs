using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// A tile that a card can be played on
public class TileInteractable : MonoBehaviour
{
    [HideInInspector] public bool occupied = false;
    [HideInInspector] public Vector2Int location; //DEPRECATED 
    [HideInInspector] public BoardCoords locationNew;
}
