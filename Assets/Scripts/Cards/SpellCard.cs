using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Allows a card to be created from the menu when right clicking in the inspector


// Spells do not occupy a space on the board when played
public abstract class SpellCard : Card
{
    [HideInInspector] public override CardInteractable CardInteractableRef { get { return SpellCardInteractableRef; } set { SpellCardInteractableRef = (SpellCardInteractable)value; } }
    public SpellCardInteractable SpellCardInteractableRef;

    public static int cloneCount = 0;

    public override Card Clone()
    {
        /*if (cloneCount > 10000)
        {
            Debug.Break();
            return null;
        }*/
        SpellCard copy = (SpellCard)ScriptableObject.CreateInstance(this.GetType());

        copy.Name = this.Name;
        copy.ManaCost = this.ManaCost;
        copy.Artwork = this.Artwork;
        copy.CurrentTeam = this.CurrentTeam;
        copy.CardInteractableRef = this.CardInteractableRef;
        copy.drawStatus = this.drawStatus;
        copy.id = this.id;

        CloneExtras(copy);

        //Debug.LogError("Spell card cloning is WIP!");
        cloneCount++;

        if (copy == null) Debug.LogError("copied spell card to null");
        return copy;
    }

    public virtual void CloneExtras(SpellCard copy) { }

    protected void FinishCast(DuelInstance duel)
    {
        duel.GetStatus(CurrentTeam).UseMana(ManaCost);
        duel.GetStatus(CurrentTeam).RemoveFromHand(this);
        duel.GetStatus(CurrentTeam).Deck.Discard(this);
        AnimationManager.Instance.SpellDiscardAnimation(duel, this);
    }


}
