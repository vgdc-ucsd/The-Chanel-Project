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

    public Card GetCardAtPos(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return null;
        return CardSlots[pos.ToRowColV2().x,pos.ToRowColV2().y];
    }

    public bool IsOutOfBounds(BoardCoords atkDest)
    {
        return atkDest.x < 0 || atkDest.x >= Cols || atkDest.y < 0 || atkDest.y >= Rows;
    }
}
