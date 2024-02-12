using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores information about the state of the board
public class Board
{
    [HideInInspector] public Card[,] CardSlots = null;
    public int Rows;
    public int Cols;

    public Board(int rows, int cols) {
        CardSlots = new Card[rows, cols];
        Rows = rows;
        Cols = cols;
    }
}
