using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores information about the state of the board
public class Board
{
    [HideInInspector] public UnitCard[,] CardSlots = null;
    public int Rows;
    public int Cols;

    public Board(int rows, int cols) {
        CardSlots = new UnitCard[rows, cols];
        Rows = rows;
        Cols = cols;
    }

    public Board Clone() {
        Board clone = new Board(Rows, Cols);
        for(int i = 0; i < Rows; i++) {
            for(int j = 0; j < Cols; j++) {
                // guarenteed to be a UnitCard
                clone.CardSlots[i, j] = (UnitCard) this.CardSlots[i, j].Clone();
            }
        }
        return clone;
    }

    public UnitCard GetCard(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return null;
        return CardSlots[pos.ToRowColV2().x,pos.ToRowColV2().y];
    }

    public UnitCard GetCard(int x, int y) // Should only use BoardCoords
    {
        return GetCard(new BoardCoords(x,y));
    }

    public bool IsOccupied(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return false;
        return !(GetCard(pos) == null);
    }

    private void SetCard(UnitCard card, BoardCoords pos)
    {
        CardSlots[pos.ToRowColV2().x, pos.ToRowColV2().y] = card;
    }

    public void PlayCard(UnitCard card, BoardCoords pos, CharStatus status, bool mainDuel) {
        if(IsOutOfBounds(pos)) {
            Debug.LogWarning("Tried to play card at out of bounds position");
            return;
        }
        if (GetCard(pos) != null) {
            Debug.LogWarning("Cannot place card at occupied tile");
            return;
        }
        if(status.Mana < card.ManaCost) {
            Debug.LogWarning("Tried to play card without enough mana");
            return;
        }

        card.Place(pos);
        status.Cards.Remove(card);
        status.UseMana(card.ManaCost);
        SetCard(card, pos);

        foreach(Ability a in card.Abilities) {
            if(a.Condition == ActivationCondition.OnPlay) a.Activate(this, card, mainDuel);
        }
    }

    public void RemoveCard(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return;
        SetCard(null, pos);
    }

    public void MoveCard(UnitCard card, BoardCoords pos, bool mainDuel)
    {
        // move card and update board and card data
        if (IsOutOfBounds(pos)) return;
        if (GetCard(pos) != null) return;
        SetCard(null, card.Pos);
        SetCard(card, pos);
        card.Pos = pos;
        card.CanMove = false;
        foreach(Ability a in card.Abilities) {
            if(a.Condition == ActivationCondition.OnMove) a.Activate(this, card, mainDuel);
        }
        card.UnitCardInteractableRef.UpdateCardPos();
        
    }

    public void RenewMovement(Team t) {
        for(int i = 0; i < Cols; i++) {
            for(int j = 0; j < Rows; j++) {
                BoardCoords pos = new BoardCoords(i,j);
                if (IsOccupied(pos)) {
                    UnitCard c = GetCard(pos);
                    if(c.CurrentTeam == t) c.CanMove = true;
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

    // size 1 = 1x1, size 2 = 3x3
    public List<BoardCoords> GetSquareTiles(BoardCoords pos, int size)
    {
        List<BoardCoords> tiles = new List<BoardCoords>();
        for (int i = -(size - 1); i <= (size - 1); i++)
        {
            for (int j = -(size-1); j <= (size-1); j++)
            {
                BoardCoords tile = pos + new BoardCoords(i, j);
                if (!IsOutOfBounds(tile)) tiles.Add(tile);
            }
        }
        return tiles;
    }

    public List<UnitCard> GetCardsInSquare(BoardCoords pos, int size)
    {
        List<UnitCard> cards = new List<UnitCard>();
        foreach (BoardCoords tile in GetSquareTiles(pos, size))
        {
            UnitCard card = GetCard(tile);
            if (card != null) cards.Add(card);
        }
        return cards;
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
