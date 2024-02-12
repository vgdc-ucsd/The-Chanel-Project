using System;
using UnityEngine;

public struct BoardCoords
{
	//(0,0) at bottom left
	//x = right
	//y = up
	public int x;
	public int y;



	public BoardCoords(int x, int y)
	{
		this.x = x; this.y = y;
	}

    public BoardCoords(Vector2Int v)
    {
        this.x = v.x;
        this.y = v.y;
    }

	public BoardCoords ToRowCol()
	{
		return new BoardCoords(DuelManager.instance.BoardRows - y - 1, x);
	}

	public Vector2Int ToRowColV2()
	{
		return new Vector2Int(DuelManager.instance.BoardRows - y - 1, x);
	}

	public static BoardCoords FromRowCol(Vector2Int boardPos)
	{
		return FromRowCol(boardPos.x, boardPos.y);
	}
    public static BoardCoords FromRowCol(int r, int c)
    {
		return new BoardCoords(c, DuelManager.instance.BoardRows - r - 1);

    }

    public override bool Equals(object obj)
    {
        return obj is BoardCoords pos &&
               x == pos.x &&
               y == pos.y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static BoardCoords operator +(BoardCoords a, BoardCoords b)
    {
        return new BoardCoords(a.x + b.x, a.y + b.y);
    }

    public static BoardCoords operator -(BoardCoords a, BoardCoords b)
    {
        return new BoardCoords(a.x - b.x, a.y - b.y);
    }

    public static BoardCoords operator *(BoardCoords a, int b)
    {
        return new BoardCoords(a.x * b, a.y * b);
    }

    public static BoardCoords operator *(BoardCoords a, float b)
    {
        return new BoardCoords(Mathf.RoundToInt(a.x * b), Mathf.RoundToInt(a.y * b));
    }

    public static BoardCoords operator *(int b, BoardCoords a)
    {
        return new BoardCoords(a.x * b, a.y * b);
    }
    public static int operator *(BoardCoords a, BoardCoords b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static bool operator ==(BoardCoords a, BoardCoords b)
    {
        return (a.x == b.x) && (a.y == b.y);
    }

    public static bool operator !=(BoardCoords a, BoardCoords b)
    {
        return !((a.x == b.x) && (a.y == b.y));
    }
}