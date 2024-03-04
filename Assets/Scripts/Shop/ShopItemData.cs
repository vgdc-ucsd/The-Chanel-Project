using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Name", menuName = "Shop Item")]
public class ShopItemData : ScriptableObject
{
    public new string name;
    public int health;
    public int attack;
    public int cost;
    public string excerpt;
    // public Sprite icon;

    public ShopItemData(string name, int health, int attack, int cost, string excerpt)
    {
        this.name = name;
        this.health = health;
        this.attack = attack;
        this.cost = cost;
        this.excerpt = excerpt;
    }
}
