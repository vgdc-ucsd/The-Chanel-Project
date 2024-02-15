using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharStatus
{
    public int Health;
    public int Mana;

    public int MaxHealth;
    public int MaxMana;

    public CharStatus(PlayerSettings settings) {
        Health = settings.StartingHealth;
        Mana = settings.StartingMana;
        MaxHealth = settings.MaxHealth;
        MaxMana = settings.MaxMana;
    }

    public void DealDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
            //LOSE
        }
    }

    public void UseMana(int manaUsed)
    {
        Mana -= manaUsed;
    }

    public void AddMana(int manaAdded)
    {
        Mana += manaAdded;
        if (Mana > MaxMana)
        {
            Mana = MaxMana;
        }
    }

    public bool canUseMana(int manaUsed)
    {
        if (manaUsed > Mana) return false;
        return true;
    }
 }
