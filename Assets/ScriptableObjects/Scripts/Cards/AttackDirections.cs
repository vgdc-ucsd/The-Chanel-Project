using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackDirections
{
    public static Vector2Int UpLeft = new Vector2Int(-1, 1); 
    public static Vector2Int Up = new Vector2Int(0, 1); 
    public static Vector2Int UpRight = new Vector2Int(1, 1); 
    public static Vector2Int Left = new Vector2Int(-1, 0); 
    public static Vector2Int Right = new Vector2Int(1, 0); 
    public static Vector2Int DownLeft = new Vector2Int(-1, -1); 
    public static Vector2Int Down = new Vector2Int(0, -1); 
    public static Vector2Int DownRight = new Vector2Int(1, -1);

    // Remember to add any new attack directions to this list
    public static List<Vector2Int> AllAttackDirections = new List<Vector2Int>{
        UpLeft, 
        Up, 
        UpRight, 
        Left, 
        Right,
        DownLeft,
        Down,
        DownRight    
    };
}
