using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




// Allows a card to be created from the menu when right clicking in the inspector
[CreateAssetMenu(fileName = "New Spell Card", menuName = "Cards/SpellCard")]

// Stores data on any given card in the game
public class SpellCard : Card
{

    // The name and stats of the card 
    
    public int Health;
    public int ManaCost;
    public Spell spell;


    public override void Place(BoardCoords pos)
    {
        if (spell is ISpellTypeTile tileSpell)
        {
            tileSpell.CastSpell(pos);

        }
        CardInteractableRef.PlaceCard(pos);
        // CardInteractableRef.PlaceSpellCard(pos);
    }

}
