using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Attack 
{
    public Vector2Int direction;
    public int damage;
    public Attack(Vector2Int direction, int damage)
    {
        this.direction = direction;
        this.damage = damage;
    }
}
