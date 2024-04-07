using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack 
{
    public Vector2Int direction;
    public int damage;
    public Card card; // TODO remove
    public Attack(Vector2Int direction, int damage, Card origin)
    {
        this.direction = direction;
        this.damage = damage;
        this.card = origin;
    }
}
