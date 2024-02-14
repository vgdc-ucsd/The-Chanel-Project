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
    public int Attack;
    public int ManaCost;

    // The image that is displayed on the card
    public Texture2D Artwork;

    // True if it's the player's card, false if it's the enemy's
    [HideInInspector] public bool BelongsToPlayer = true;

    // References to card and tile interactables
    [HideInInspector] public CardInteractable CardInteractableRef;
    [HideInInspector] public TileInteractable TileInteractableRef;

    // The directions that the card attacks when facing right.
    // Hidden because values here are set through the custom editor
    [HideInInspector] public List<Vector2Int> AttackDirections = new List<Vector2Int>();

}
