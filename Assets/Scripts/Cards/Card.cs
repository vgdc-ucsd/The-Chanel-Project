using UnityEngine;
using System.Collections.Generic;

public abstract class Card: ScriptableObject
{
    public string Name;
    public int ManaCost;
    public Texture2D Artwork;
    
    [HideInInspector] public Team CurrentTeam = Team.Neutral;
    [HideInInspector] public abstract CardInteractable CardInteractableRef { get; }

    public abstract Card Clone();
}
