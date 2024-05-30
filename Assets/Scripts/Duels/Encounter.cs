using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter")]
public class Encounter : ScriptableObject
{
    public String EncounterName;
    public DuelSettings Settings;
    public Sprite EnemyArt;
    public Deck[] EnemyDecks;
    public Deck EnemyDeck;
    public int RewardGold;
    public Card[] CardOffers; // you can choose one if you win
    public BossData boss = null;
    public FMODUnity.StudioEventEmitter EncounterAudio;
    public FMODUnity.StudioEventEmitter WinAudio;
    public FMODUnity.StudioEventEmitter LoseAudio;

    [Header("Slightly affects AI behavior")]
    [Header("Aggression: 0.5-0.9, Defense: 0.5-0.7")]
    public float AIAggression = 0.7f;   
    public float AIDefense = 0.6f;      
}
