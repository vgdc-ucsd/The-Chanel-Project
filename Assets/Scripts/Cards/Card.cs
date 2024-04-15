using UnityEngine;

public abstract class Card: ScriptableObject
{
    [HideInInspector] public CardInteractable CardInteractableRef;
    public string Name;
    // The image that is displayed on the card
    public Texture2D Artwork;
    public Team team = Team.Neutral;
    public abstract void Place(BoardCoords pos);
}