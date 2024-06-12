using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
                if(this.CardSlots[i, j] != null) {
                    clone.CardSlots[i, j] = (UnitCard) this.CardSlots[i, j].Clone();
                }
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

    // this should only be called on valid placements
    public void PlayCard(UnitCard card, BoardCoords pos, CharStatus status, DuelInstance duel) {
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

        card.Place(pos, duel);
        if (!status.Cards.Remove(card)) Debug.LogError("Failed to remove card");
        status.UseMana(card.ManaCost);
        SetCard(card, pos);
        // TODO mana spend animation
        // AnimationManager.Instance.UpdateUIAnimation(duel);

        ActivationInfo info = new ActivationInfo(duel);
        foreach(Ability a in card.Abilities) {
            if(a.Condition == ActivationCondition.OnPlay) a.Activate(card, info);
        }
    }

    public void RemoveCard(BoardCoords pos)
    {
        if (IsOutOfBounds(pos)) return;
        SetCard(null, pos);
    }

    public void MoveCard(UnitCard card, BoardCoords pos, DuelInstance duel, bool pushed = false)
    {
        // move card and update board and card data
        if (IsOutOfBounds(pos)) return;
        if (GetCard(pos) != null) return;
        SetCard(null, card.Pos);
        SetCard(card, pos);
        card.Pos = pos;
        if (!pushed)
        {
            card.CanMove = false;
            IEnumerator ie = AnimationManager.Instance.CardCantMove(card);
            QueueableAnimation qa = new QueueableAnimation(ie, 1.0f);
            duel.Animations.Enqueue(qa);
            ActivationInfo info = new ActivationInfo(duel);
            foreach (Ability a in card.Abilities)
            {
                if (a.Condition == ActivationCondition.OnMove) a.Activate(card, info);
            }
        }

        AnimationManager.Instance.MoveCardAnimation(duel, card, pos);
        //if (duel == DuelManager.Instance.MainDuel) card.UnitCardInteractableRef.UpdateCardPos();

    }

    public void TeleportCard(UnitCard card, BoardCoords pos, DuelInstance duel, bool clearPrev = true)
    {
        // move card and update board and card data
        if (IsOutOfBounds(pos)) return;
        if (clearPrev) SetCard(null, card.Pos);
        SetCard(card, pos);
        card.Pos = pos;
        AnimationManager.Instance.MoveCardAnimation(duel, card, pos);
    }

<<<<<<< Updated upstream
    public void RenewMovement(Team t) {
        for(int i = 0; i < Cols; i++) {
            for(int j = 0; j < Rows; j++) {
                BoardCoords pos = new BoardCoords(i,j);
                if (IsOccupied(pos)) {
=======
    public void RenewMovement(Team t, DuelInstance duel)
    {
        for (int i = 0; i < Cols; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                BoardCoords pos = new BoardCoords(i, j);
                if (IsOccupied(pos))
                {
>>>>>>> Stashed changes
                    UnitCard c = GetCard(pos);
                    if (c.CurrentTeam == t)
                    {
                        c.CanAttack = true;
                        c.CanMove = true;
                        IEnumerator ie = AnimationManager.Instance.CardCanMove(c);
                        QueueableAnimation qa = new QueueableAnimation(ie, 1.0f);
                        duel.Animations.Enqueue(qa);
                    }
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

    public List<UnitCard> GetAdjacentCards(BoardCoords pos)
    {
        List<UnitCard> cards = new List<UnitCard>();
        foreach (BoardCoords tile in GetAdjacentTiles(pos))
        {
            UnitCard card = GetCard(tile);
            if (card != null) cards.Add(card);
        }
        return cards;
    }
    public BoardCoords GetFrontTile(BoardCoords pos,Team team)
    {

        BoardCoords result;
        if (team == Team.Player) 
        result = pos + new BoardCoords(0, 1);
        else
            result = pos + new BoardCoords(0, -1);
        return result;
    }

    public UnitCard GetFrontCard(BoardCoords pos, Team team)
    {
        BoardCoords tile = GetFrontTile(pos, team);
        UnitCard card = GetCard(tile);
        return card;
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

    public List<UnitCard> GetAllCards()
    {
        List<UnitCard> cards = new List<UnitCard>();
        foreach (UnitCard card in CardSlots)
        {
            if (card != null) cards.Add(card);
        }
        return cards;
    }

    public List<UnitCard> GetCardsOfTeam(Team team)
    {
        List<UnitCard> cards = new List<UnitCard>();
        foreach (UnitCard card in CardSlots)
        {
            if (card != null && card.CurrentTeam == team) cards.Add(card);
        }
        return cards;
    }

    public List<UnitCard> GetCardsInRow(int y)
    {
        if (IsOutOfBounds(new BoardCoords(0,y)))
        {
            return null;
        }
        List<UnitCard> cards = new List<UnitCard> ();
        for (int i = 0; i < Cols; i++)
        {
            UnitCard card = GetCard(i, y);
            if (card != null) cards.Add(card);
        }
        return cards;
    }

    public List<UnitCard> GetCardsInColumn(int x)
    {
        if (IsOutOfBounds(new BoardCoords(x, 0)))
        {
            return null;
        }
        List<UnitCard> cards = new List<UnitCard>();
        for (int i = 0; i < Rows; i++)
        {
            UnitCard card = GetCard(x, i);
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

    public BoardCoords GetRandomTile()
    {
        int xTile = Random.Range(0, Cols);
        int yTile = Random.Range(0, Rows);
        return new BoardCoords(xTile, yTile);   
    }

    public UnitCard GetRandomCard()
    {
        List<UnitCard> cards = GetAllCards();
        if (cards.Count == 0) return null;
        return cards[Random.Range(0, cards.Count)];
    }

    public UnitCard GetRandomCardOfTeam(Team team)
    {
        List<UnitCard> cards = GetCardsOfTeam(team);
        if (cards.Count == 0) return null;
        return cards[Random.Range(0, cards.Count)];
    }
}
