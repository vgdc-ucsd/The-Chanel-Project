using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Name", menuName = "Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public new string name;
    public int health;
    public int attack;
    // public Sprite icon;

    public InventoryItemData(string name, int health, int attack)
    {
        this.name = name;
        this.health = health;
        this.attack = attack;
    }
}
