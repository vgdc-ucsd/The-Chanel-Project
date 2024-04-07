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
        DuelEvents.Instance.OnPlaceCard += PlaceCard;
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

    private void SetCard(Card card, BoardCoords pos)
    {
        CardSlots[pos.ToRowColV2().x, pos.ToRowColV2().y] = card;
    }

    public void PlaceCard(Card card, BoardCoords pos) 
    {
        if (IsOutOfBounds(pos)) return;
        if (GetCard(pos) != null) return; //cannot place card at occupied tile
        SetCard(card, pos);
        card.pos = pos;
        card.Place(pos);
        return;
    }

    public void PlaceCard(Card card, BoardCoords pos, Team team) // ?
    {
        PlaceCard(card, pos);
    }

    public void RemoveCard(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return;
        SetCard(null, pos);
    }

    public void MoveCard(Card card, BoardCoords pos)
    {
        // move card and update board and card data
        if (IsOutOfBounds(pos)) return;
        if (GetCard(pos) != null) return;
        SetCard(null, card.pos);
        SetCard(card, pos);
        card.pos = pos;
        card.CanMove = false;
        foreach(Ability a in card.Abilities) {
            if(a.Condition == ActivationCondition.OnMove) a.Activate(card);
        }
    }

    public void RenewMovement(Team t) {
        for(int i = 0; i < Cols; i++) {
            for(int j = 0; j < Rows; j++) {
                BoardCoords pos = new BoardCoords(i,j);
                if (IsOccupied(pos)) {
                    Card c = GetCard(pos);
                    if(c.team == t) c.CanMove = true;
                }
            }
        }
    }

    public List<BoardCoords> GetAdjacentTiles(BoardCoords pos)
    {
        List<BoardCoords> tiles = new List<BoardCoords>();
        BoardCoords up = pos + new BoardCoords(0, 1);
        if (!IsOutOfBounds(up)) tiles.Add(up);
        BoardCoords right = pos + new BoardCoords(1, 0);
        if (!IsOutOfBounds(right)) tiles.Add(right);
        BoardCoords down = pos + new BoardCoords(0, -1);
        if (!IsOutOfBounds(down)) tiles.Add(down);
        BoardCoords left = pos + new BoardCoords(-1, 0);
        if (!IsOutOfBounds(left)) tiles.Add(left);

        return tiles;
    }
    
    public List<BoardCoords> GetEmptyAdjacentTiles(BoardCoords pos)
    {
        List<BoardCoords> tiles = GetAdjacentTiles(pos);
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if (IsOccupied(tiles[i])) tiles.RemoveAt(i);
        }
        return tiles;
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
