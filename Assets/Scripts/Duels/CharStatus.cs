using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class CharStatus
{
    public int Health;
    public int Mana;
    public bool IsAlive = true;
    public int MaxHealth;
    public int MaxMana;
    public int ManaCapacity;
    public Team CharTeam;
    public Deck Deck;
    public List<Card> Cards = new List<Card>();

    PlayerSettings playerSettings;
    DuelSettings duelSettings;

    public CharStatus(Team team, Deck deck) {
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
        ManaCapacity = playerSettings.StartingMana;
        this.CharTeam = team;
        Deck = deck;
        Cards = new List<Card>();
    }

    private CharStatus() {}

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
            Card cc = c.Clone();
            if (cc ==  null) 
            {
                Debug.LogError("added null clone to CharStatus");
            }
            copy.Cards.Add(cc);
            
        }
        copy.playerSettings = this.playerSettings;
        copy.duelSettings = this.duelSettings;
        return copy;
    }

    public void AddCard(Card c) {
        c.CurrentTeam = CharTeam;
        if(c.CurrentTeam == Team.Enemy) {
            if(c.GetType() == typeof(UnitCard)) {
                UnitCard unitCard = (UnitCard) c;
                foreach(Attack atk in unitCard.Attacks) {
                    atk.direction.y *= -1;
                }
            }
        }
        Cards.Add(c);

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

    public Team TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            IsAlive = false;
            if (CharTeam == Team.Player)
            {
                return Team.Enemy;
                // TODO load scene as queueable animation

                /* Debug.Log("Enemy Won!");
                if (MenuScript.Instance != null)
                {
                    MenuScript.Instance.LoadMap(); // Transitions into map - Kiichi
                    Debug.Log("SceneManager not Present: Failed to Load Map");
                } */
            }
            else
            {
                return Team.Player;
                /* Debug.Log("Player Won!");
                if (MenuScript.Instance != null)
                {
                    MenuScript.Instance.LoadMap(); // Transitions into map - Kiichi
                    Debug.Log("SceneManager not Present: Failed to Load Map");
                } */
            }
        }
        return Team.Neutral;
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

    // can expend mana to draw a card
    public bool CanDrawCard()
    {
        return (!Deck.DrawPileIsEmpty() && CanUseMana(duelSettings.DrawCardManaCost));
    }
    public static Team OppositeTeam(Team team)
    {
        switch (team)
        {
            case Team.Player:
                return Team.Enemy;
            case Team.Enemy:
                return Team.Player;
            default:
                return Team.Neutral;
        }
    }
 }
