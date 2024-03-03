using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop Inventory List")]
public class ShopList : ScriptableObject
{
    public List<Card> CardList;
    public Deck deck;
}
