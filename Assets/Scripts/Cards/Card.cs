using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// Allows a card to be created fropm the menu when right clicking in the inspector
[CreateAssetMenu(menuName = "Cards/Card")]

// Stores data on any given card in the game
public class Card : ScriptableObject
{
    // The name and stats of the card 
    public string Name;
    public int Health;
    public int AttackDamage; // OLD
    public int ManaCost;

    public BoardCoords pos;
    [HideInInspector] public bool isActive = false;

    // Directions: upleft, up, upright, left, right, downleft, down, downright
    // Someone please replace this soon, this is disgusting
    public int[] AttackDamages = new int[8] 
        {-1,-1,-1,
         -1,   -1,
         -1,-1,-1};

    public List<Attack> Attacks = new List<Attack>();
    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            if (AttackDamages[i] != -1)
            {
                Attacks.Add(new Attack(global::AttackDirections.AllAttackDirections[i], AttackDamages[i], this));
            }
        }
    }

    public Attack GetAttack(Vector2Int dir)
    {
        foreach (Attack attack in Attacks)
        {
            if (attack.direction == dir) return attack;
        }
        return null;
    }

    public void DealDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            TileInteractableRef.occupied = false;
            DuelManager.Instance.DC.GetCurrentBoard().RemoveCard(TileInteractableRef.location);
            MonoBehaviour.Destroy(CardInteractableRef.gameObject);
        }
    }

    public void Place(BoardCoords pos)
    {
        isActive = true;
        CardInteractableRef.PlaceCard(pos);
    }

    // The image that is displayed on the card
    public Texture2D Artwork;

    // True if it's the player's card, false if it's the enemy's
    [HideInInspector] public Team team = Team.Player;

    // References to card and tile interactables
    [HideInInspector] public CardInteractable CardInteractableRef;
    [HideInInspector] public TileInteractable TileInteractableRef;

    // The directions that the card attacks when facing right.
    // Hidden because values here are set through the custom editor
    [HideInInspector] public List<Vector2Int> AttackDirections = new List<Vector2Int>();
    // TODO: Rename this to solve name conflict

}
