using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter")]
public class Encounter : ScriptableObject
{
    public DuelSettings Settings;
    public Sprite EnemyArt;
    public int RewardGold;
    public Card[] CardOffers; // you can choose one if you win
}
