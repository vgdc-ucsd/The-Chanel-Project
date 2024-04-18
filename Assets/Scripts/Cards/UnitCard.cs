using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


// Allows a card to be created from the menu when right clicking in the inspector
[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]

// Stores data on any given card in the game
public class UnitCard : Card
{
    public int Health;
    
    [HideInInspector] public bool CanMove = true;
    [HideInInspector] public BoardCoords Pos;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public int BaseDamage = 1; // set in the custom card editor
    [HideInInspector] public override CardInteractable CardInteractableRef { get {return UnitCardInteractableRef;} }
    public UnitCardInteractable UnitCardInteractableRef;

    public List<Attack> Attacks = new List<Attack>();
    public List<Ability> Abilities = new List<Ability>();

    public override Card Clone() {
        UnitCard copy = (UnitCard) ScriptableObject.CreateInstance("UnitCard");

        copy.Name = this.Name;
        copy.BaseDamage = this.BaseDamage;
        copy.Health = this.Health;
        copy.ManaCost = this.ManaCost;
        copy.isSelected = false;
        copy.CanMove = this.CanMove;
        copy.Pos = this.Pos;
        copy.Artwork = null; // maybe
        copy.CurrentTeam = this.CurrentTeam;
        copy.UnitCardInteractableRef = null;

        copy.Attacks = new List<Attack>();
        foreach(Attack atk in this.Attacks) {
            copy.Attacks.Add(new Attack(atk.direction, atk.damage));
        }
        copy.Abilities = new List<Ability>();
        foreach(Ability ab in this.Abilities) {
            copy.Abilities.Add(ab);
        }

        return copy;
    }

    public TileInteractable GetTile()
    {
        return BoardInterface.Instance.Tiles[Pos.ToRowColV2().x, Pos.ToRowColV2().y];
    }

    public Attack GetAttack(Vector2Int dir)
    {
        foreach (Attack attack in Attacks)
        {
            if (attack.direction == dir) return attack;
        }
        return null;
    }

    public void Place(BoardCoords pos)
    {
        this.Pos = pos;
        CanMove = true;
        if( UnitCardInteractableRef != null) UnitCardInteractableRef.PlaceCard(pos);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        CardInteractableRef.SetSelected(selected); // TODO
    }
}
