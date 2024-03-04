using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// Allows a card to be created fropm the menu when right clicking in the inspector
[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]

// Stores data on any given card in the game
public class Card : ScriptableObject
{
    public bool enableLogging;

    // The name and stats of the card 
    public string Name;
    public int Health;
    public int AttackDamage; // OLD
    public int ManaCost;
    public bool isSelected = false;

    public BoardCoords pos;
    [HideInInspector] public bool isActive = false;

    // The image that is displayed on the card
    public Texture2D Artwork;

    public Team team = Team.Neutral;

    // References to card and tile interactables
    [HideInInspector] public CardInteractable CardInteractableRef;


    //[HideInInspector] public TileInteractable TileInteractableRef;
    // Removed - too annoying to keep updating the tile interactable ref when card moves
    // use BoardInterface.Instance.GetTile(card.pos)

    // The directions that the card attacks when facing right.
    // Hidden because values here are set through the custom editor
    [HideInInspector] public List<Vector2Int> AttackDirections = new List<Vector2Int>();

    // Directions: upleft, up, upright, left, right, downleft, down, downright
    // Someone please replace this soon, this is disgusting
    public int[] AttackDamages = new int[8] 
        {-1,-1,-1,
         -1,   -1,
         -1,-1,-1};

    public List<Attack> Attacks = new List<Attack>();
    public List<Ability> Abilities = new List<Ability>();

    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            if (AttackDamages[i] != -1)
            {
                Attacks.Add(new Attack(global::AttackDirections.AllAttackDirections[i], AttackDamages[i], this));
            }
        }
    }

    

    public TileInteractable GetTile()
    {
        return BoardInterface.Instance.Tiles[pos.ToRowColV2().x, pos.ToRowColV2().y];
    }

    public Attack GetAttack(Vector2Int dir)
    {
        foreach (Attack attack in Attacks)
        {
            if (attack.direction == dir) return attack;
        }
        return null;
    }

    public void DealDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            DuelManager.Instance.DC.GetCurrentBoard().RemoveCard(pos);
            IEnumerator ie = DuelManager.Instance.AM.CardDeath(CardInteractableRef);
            QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
            DuelManager.Instance.AM.QueueAnimation(qa);
        }
    }

    public void Place(BoardCoords pos)
    {
        isActive = true;
        CardInteractableRef.PlaceCard(pos);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        CardInteractableRef.SetSelected(selected);
    }





}
