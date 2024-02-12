using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
public class Inventory : ScriptableObject
{
    [HideInInspector] public Card[,] CardSlots = null;
    public int Rows;
    public int Cols;

    public Inventory(int rows, int cols)
    {
        CardSlots = new Card[rows, cols];
        Rows = rows;
        Cols = cols;
    }

}
