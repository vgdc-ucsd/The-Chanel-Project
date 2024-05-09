using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Attack 
{
    public Vector2Int direction;
    public int damage;

    public Attack(Vector2Int direction, int damage)
    {
        this.direction = direction;
        this.damage = damage;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null) return false;

        Attack atk = obj as Attack;
        if ((System.Object)atk == null) return false;

        return (this.damage == atk.damage) && (this.direction == atk.direction);
    }

    public bool Equals(Attack atk)
    {
        if ((object)atk == null) return false;
        return (this.damage == atk.damage) && (this.direction == atk.direction);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static List<Vector2Int> allDirections = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1)
    }.ToList<Vector2Int>();
}
