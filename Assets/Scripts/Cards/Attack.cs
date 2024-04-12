using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack 
{
    public Vector2Int direction;
    public int damage;
    public UnitCard card; // TODO remove
    public Attack(Vector2Int direction, int damage, UnitCard origin)
    {
        this.direction = direction;
        this.damage = damage;
        this.card = origin;
    }
}
