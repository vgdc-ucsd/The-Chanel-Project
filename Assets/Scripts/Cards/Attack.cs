using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack 
{
    public Vector2Int direction;
    public int damage;
    public Card card;
    public Attack(Vector2Int direction, int damage, Card origin)
    {
        this.direction = direction;
        this.damage = damage;
        this.card = origin;
    }

    public void Hit(Card card)
    {
        card.DealDamage(damage);
        //additional effects..
    }
}
