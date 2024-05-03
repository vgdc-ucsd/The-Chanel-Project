using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Allows a card to be created from the menu when right clicking in the inspector
[CreateAssetMenu(fileName = "New Spell Card", menuName = "Cards/SpellCard")]

// Spells do not occupy a space on the board when played
public class SpellCard : Card
{
    public override CardInteractable CardInteractableRef { get; set; }

    public override Card Clone()
    {
        throw new System.NotImplementedException();
    }
}
