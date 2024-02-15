using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharStatus
{
    public int Health { get; private set; }
    public int Mana { get; private set; }

    public int MaxHealth;
    public int MaxMana;
    public int ManaRegen;

    public CharStatus(PlayerSettings settings) {
        Health = settings.StartingHealth;
        Mana = settings.StartingMana;
        MaxHealth = settings.MaxHealth;
        MaxMana = settings.MaxMana;
        ManaRegen = settings.ManaRegen;
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

    public void RegenMana()
    {
        AddMana(ManaRegen);
    }

    public bool CanUseMana(int manaUsed)
    {
        if (manaUsed > Mana) return false;
        return true;
    }
 }
