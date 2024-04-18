using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class CharStatus
{
    public int Health { get; private set; }
    public int Mana { get; private set; }
    public bool IsAlive = true;
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public int MaxMana;
    [HideInInspector] public int ManaCapacity;
    public Team CharTeam;
    public Deck Deck;
    public List<Card> Cards = new List<Card>();


    //[SerializeField] 
    PlayerSettings playerSettings;
    DuelSettings duelSettings;

    public CharStatus(Team team) {
        duelSettings = DuelManager.Instance.Settings;
        if(team == Team.Player || duelSettings.SameSettingsForBothPlayers) {
            playerSettings = duelSettings.Player;
        }
        else {
            playerSettings = duelSettings.Enemy;
        }

        Mana = playerSettings.StartingMana;
        Health = playerSettings.StartingHealth;
        IsAlive = true;
        MaxHealth = playerSettings.MaxHealth;
        MaxMana = playerSettings.MaxMana;
        ManaCapacity = 1; // double check
        this.CharTeam = team;
        if(team == Team.Player) {
            Deck = DuelManager.Instance.PlayerDeck;
        }
        else {
            Deck = DuelManager.Instance.EnemyDeck;
        }
        Cards = new List<Card>();
    }

    public CharStatus() {
        
    }

    //private void Awake()
    //{
        //currently settings are set at duel manager, can change to set here if desired
        /*
        Health = settings.StartingHealth;
        Mana = settings.StartingMana;
        MaxHealth = settings.MaxHealth;
        MaxMana = settings.MaxMana;
        ManaRegen = settings.ManaRegen;
        */
    //    duelSettings = DuelManager.Instance.Settings;
    //    DuelEvents.Instance.OnDrawCard += AddCard;
    //    DuelEvents.Instance.OnRemoveFromHand += RemoveFromHand;
    //    DuelEvents.Instance.OnAdvanceGameTurn += GiveMana;
    //}

    public void Init(Team team)
    {
        this.CharTeam = team;
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

    public CharStatus Clone() {
        CharStatus copy = new CharStatus();
        copy.Mana = this.Mana;
        copy.Health = this.Health;
        copy.IsAlive = this.IsAlive;
        copy.MaxHealth = this.MaxHealth;
        copy.MaxMana = this.MaxMana;
        copy.ManaCapacity = this.ManaCapacity;
        copy.CharTeam = this.CharTeam;
        copy.Deck = this.Deck.Clone();
        copy.Cards = new List<Card>();
        foreach(Card c in this.Cards) {
            copy.Cards.Add(c.Clone());
        }
        copy.playerSettings = this.playerSettings;
        copy.duelSettings = this.duelSettings;
        return copy;
    }

    public void AddCard(Card card, Team team)
    {
        if (team == this.CharTeam) 
        { 
            Cards.Add(card);
            card.CurrentTeam = team;
        }
        
    }

    public void RemoveFromHand(Card card)
    {
        if (!Cards.Contains(card)) 
        {
            //Debug.Log($"Tried to remove {card.Name} but was not in hand");
            return;
        }
        Cards.Remove(card);
    }

    public void DealDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            IsAlive = false;
            if (CharTeam == Team.Player)
            {
                Debug.Log("Enemy Won!");
                if (MenuScript.Instance != null)
                {
                    MenuScript.Instance.LoadMap(); // Transitions into map - Kiichi
                    Debug.Log("SceneManager not Present: Failed to Load Map");
                }
            }
            else
            {
                Debug.Log("Player Won!");
                if (MenuScript.Instance != null)
                {
                    MenuScript.Instance.LoadMap(); // Transitions into map - Kiichi
                    Debug.Log("SceneManager not Present: Failed to Load Map");
                }
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

    public void ResetMana()
    {
        Mana = MaxMana;
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
