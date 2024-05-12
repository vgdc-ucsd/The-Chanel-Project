using UnityEngine;
using System.Collections.Generic;


public abstract class Card: ScriptableObject
{
    public string Name;
    public int ManaCost;
    public int ShopCost;
    public Sprite Artwork;
    public DrawStatus drawStatus = DrawStatus.Available;
    public int id;



    [HideInInspector] public Team CurrentTeam = Team.Neutral;
    [HideInInspector] public abstract CardInteractable CardInteractableRef { get; set; }

    public abstract Card Clone();

    public override string ToString()
    {
        return Name;
    }
}
