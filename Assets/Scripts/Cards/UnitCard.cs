using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;




public struct UnitStats
{
    public int baseDamage;
    public int health;
    public List<Attack> attacks;
}

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
    [HideInInspector] public override CardInteractable CardInteractableRef { get {return UnitCardInteractableRef;} set{UnitCardInteractableRef = (UnitCardInteractable)value;} }
    public UnitCardInteractable UnitCardInteractableRef;


    public List<Attack> Attacks = new List<Attack>();
    public List<Ability> Abilities = new List<Ability>();

    public List<StatusEffect> StatusEffects = new List<StatusEffect>(); // for effect stacking calculations, order preserved
    public UnitStats baseStats = new UnitStats();


    public override Card Clone() {
        UnitCard copy = (UnitCard) ScriptableObject.CreateInstance("UnitCard");

        copy.Name = this.Name;
        copy.BaseDamage = this.BaseDamage;
        copy.Health = this.Health;
        copy.ManaCost = this.ManaCost;
        copy.isSelected = false;
        copy.CanMove = this.CanMove;
        copy.Pos = this.Pos;
        copy.Artwork = this.Artwork;
        copy.CurrentTeam = this.CurrentTeam;
        copy.UnitCardInteractableRef = this.UnitCardInteractableRef;
        copy.baseStats = this.baseStats;

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

    public void TakeDamage(DuelInstance duel, int damage) {
        Health -= damage;
        ActivationInfo info = new ActivationInfo(duel);
        info.TotalDamage = damage;
        if(Health < 0) info.OverkillDamage = Health*-1;

        // On receive damage but still alive
        if (Health > 0) {
            AnimationManager.Instance.UpdateCardInfoAnimation(duel, this);
            foreach (Ability a in Abilities) {      
                if(a.Condition == ActivationCondition.OnReceiveDamage) a.Activate(this, info);
            }
        }
        // On death
        else {
            foreach(Ability a in Abilities) {
                if (a.Condition == ActivationCondition.OnDeath) a.Activate(this, info);
            }
        }
    }

    public void Place(BoardCoords pos, DuelInstance duel)
    {
        this.Pos = pos;
        CanMove = true;

        baseStats.health = this.Health;
        List<Attack> atkList = new List<Attack>();
        foreach (Attack atk in Attacks)
        {
            atkList.Add(new Attack(atk.direction, atk.damage));
        }
        baseStats.attacks = atkList;
        baseStats.baseDamage = this.BaseDamage;

        if(CurrentTeam == Team.Enemy) {
            AnimationManager.Instance.PlaceUnitCardAnimation(duel, this, pos);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        CardInteractableRef.SetSelected(selected); // TODO
    }

    public void RecalculateStats()
    {
        Attacks.Clear();
        foreach (Attack atk in baseStats.attacks)
        {
            Attacks.Add(new Attack(atk.direction, atk.damage));
        }
        BaseDamage = baseStats.baseDamage;

        foreach (StatusEffect effect in StatusEffects)
        {
            effect.ReapplyEffect(this);
        }

    }
    
}
