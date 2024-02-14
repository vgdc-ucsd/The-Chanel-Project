using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSettings
{
    // Health
    public int StartingHealth = 10;
    public int MaxHealth = 10;
    public bool MaxHealthEnabled = false;

    // Mana
    public int StartingMana = 0;
    public int MaxMana = 4;
    public bool MaxManaEnabled = true;

    // Cards
    public int StartingCards = 4;
}
