using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Allows a card to be created from the menu when right clicking in the inspector
[CreateAssetMenu(fileName = "New Spell Card", menuName = "Cards/SpellCard")]

// Spells do not occupy a space on the board when played
public abstract class SpellCard : Card
{
    public override CardInteractable CardInteractableRef { get; set; }

    public override Card Clone()
    {
        SpellCard copy = (SpellCard)ScriptableObject.CreateInstance(this.GetType());

        copy.Name = this.Name;
        copy.ManaCost = this.ManaCost;
        copy.Artwork = this.Artwork;
        copy.CurrentTeam = this.CurrentTeam;
        copy.CardInteractableRef = this.CardInteractableRef;

        //Debug.LogWarning("Spell card cloning is WIP!");

        return copy;
    }


}
