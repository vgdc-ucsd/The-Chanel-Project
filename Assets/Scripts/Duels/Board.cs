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

    public Card GetCard(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return null;
        return CardSlots[pos.ToRowColV2().x,pos.ToRowColV2().y];
    }

    public bool IsOccupied(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return false;
        return !(GetCard(pos) == null);
    }

    public bool PlaceCard(Card card,BoardCoords pos, bool force = false) //force for some weird effects
    {
        if (IsOutOfBounds(pos)) return false;
        if (!force)
        {
            if (GetCard(pos) != null) return false; //cannot place card at occupied tile unless forced
        }
        CardSlots[pos.ToRowColV2().x, pos.ToRowColV2().y] = card;
        return true;
    }

    public void RemoveCard(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return;
        CardSlots[pos.ToRowColV2().x, pos.ToRowColV2().y] = null;
    }

    public bool OnEnemyEdge(BoardCoords pos)
    {
        return (pos.y == Rows - 1);
    }
    public bool BeyondEnemyEdge(BoardCoords pos)
    {
        return (pos.y > Rows - 1);
    }

    public bool OnPlayerEdge(BoardCoords pos)
    {
        return (pos.y == 0);
    }
    public bool BeyondPlayerEdge(BoardCoords pos)
    {
        return (pos.y < 0);
    }

    public bool IsOutOfBounds(BoardCoords atkDest)
    {
        return atkDest.x < 0 || atkDest.x >= Cols || atkDest.y < 0 || atkDest.y >= Rows;
    }
}
