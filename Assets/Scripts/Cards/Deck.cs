using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    public List<Card> CardList = new List<Card>();

    private void addCard(Card card)
    {
        CardList.Add(card);
    }
}
