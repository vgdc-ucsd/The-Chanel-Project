using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharStatus : MonoBehaviour
{
    public int health;
    public int mana;

    const int MAX_HEALTH = 10;
    const int MAX_MANA = 4;

    [SerializeField]
    TMP_Text healthDisplay, manaDisplay;

    private void Awake()
    {
        DuelEvents.instance.onUpdateUI += UpdateStatDisplay;
    }

    // Start is called before the first frame update
    void Start()
    {
        health = MAX_HEALTH;
        mana = MAX_MANA;
    }

    void UpdateStatDisplay()
    {
        healthDisplay.text = health.ToString();
        manaDisplay.text = mana.ToString();
    }

    public void DealDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
            //LOSE
        }
    }

    public void UseMana(int manaUsed)
    {
        mana -= manaUsed;
    }

    public void AddMana(int manaAdded)
    {
        mana += manaAdded;
        if (mana > MAX_MANA)
        {
            mana = MAX_MANA;
        }
    }

    public bool canUseMana(int manaUsed)
    {
        if (manaUsed > mana) return false;
        return true;
    }
 }
