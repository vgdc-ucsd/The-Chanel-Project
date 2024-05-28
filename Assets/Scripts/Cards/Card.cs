using UnityEngine;
using System.Collections.Generic;

public enum CardType
{
    Weak, Medium, Strong, Spell
}

public abstract class Card: ScriptableObject
{
    public string Name;
    public int ManaCost;
    public int ShopCost = 50;
    public Sprite Artwork;
    public DrawStatus drawStatus = DrawStatus.Available;
    public int id;

    [TextArea(2,10)]
    public string description;

    [HideInInspector] public Team CurrentTeam = Team.Neutral;
    [HideInInspector] public abstract CardInteractable CardInteractableRef { get; set; }

    public CardType cardType;
    public abstract Card Clone();

    public override string ToString()
    {
        return Name;
    }

    public abstract CardInteractable GenerateCardInteractable();
}
