using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Deck List")]
public class InventoryList : ScriptableObject
{
    public List<Card> CardList;
    public Deck deck;
}
