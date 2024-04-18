using System.Collections;
using System.Collections.Generic;
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
}
