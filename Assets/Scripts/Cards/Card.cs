using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Allows a card to be created fropm the menu when right clicking in the inspector
[CreateAssetMenu]
public class Card : ScriptableObject
{
    // The name of the card to be displayed 
    public string Name;
    // The image that is displayed on the card
    public Texture2D Artwork;
    // The directions that the card attacks when facing right.
    [HideInInspector] // Hidden because values here are set through the custom editor
    public List<Vector2Int> AttackDirections = new List<Vector2Int>();

}
