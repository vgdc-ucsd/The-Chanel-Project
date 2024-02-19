using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class CharStatus : MonoBehaviour
{
    public int Health { get; private set; }
    public int Mana { get; private set; }
    public bool isAlive = true;
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public int MaxMana;
    [HideInInspector] public int ManaCapacity;
    private Team team;
    public Deck Deck;
    private List<Card> cards = new List<Card>();


    //[SerializeField] 
    PlayerSettings playerSettings;
    DuelSettings duelSettings;
    private void Awake()
    {
        //currently settings are set at duel manager, can change to set here if desired
        /*
        Health = settings.StartingHealth;
        Mana = settings.StartingMana;
        MaxHealth = settings.MaxHealth;
        MaxMana = settings.MaxMana;
        ManaRegen = settings.ManaRegen;
        */
        duelSettings = DuelManager.Instance.Settings;
        DuelEvents.Instance.OnDrawCard += AddCard;
        DuelEvents.Instance.OnRemoveFromHand += RemoveFromHand;
        DuelEvents.Instance.OnAdvanceGameTurn += GiveMana;
    }

    public void Init(Team team)
    {
        this.team = team;
        if (team == Team.Player || duelSettings.SameSettingsForBothPlayers)
        {
            playerSettings = DuelManager.Instance.Settings.Player;
        }
        else
        {
            playerSettings = DuelManager.Instance.Settings.Enemy;
        }
        Health = playerSettings.StartingHealth;
        Mana = playerSettings.StartingMana;
        MaxHealth = playerSettings.MaxHealth;
        MaxMana = playerSettings.MaxMana;
        ManaCapacity = 1;
    }

    public void AddCard(Card card, Team team)
    {
        if (team == this.team) { }
        cards.Add(card);
    }

    public void RemoveFromHand(Card card)
    {
        if (!cards.Contains(card)) 
        {
            //Debug.Log($"Tried to remove {card.Name} but was not in hand");
            return;
        }
        cards.Remove(card);
    }

    public void DealDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            isAlive = false;
            if (team == Team.Player)
            {
                Debug.Log("Enemy Won!");
            }
            else
            {
                Debug.Log("Player Won!");
            }
        }
    }

    public void SetDeck(Deck deck)
    {
        Deck = deck;
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

    public void GiveMana()
    {
        if (ManaCapacity < MaxMana)
        {
            ManaCapacity++;
        }
        Mana = ManaCapacity;
    }

    public bool CanUseMana(int manaUsed)
    {
        if (manaUsed > Mana) return false;
        return true;
    }
 }
