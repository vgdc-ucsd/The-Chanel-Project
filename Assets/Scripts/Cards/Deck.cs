using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    public List<Card> CardList = new List<Card>();

    private void addCard(UnitCard card)
    {
        CardList.Add(card);
    }

    public Deck Clone() {
        Deck copy = (Deck) ScriptableObject.CreateInstance("Deck");

        foreach(Card c in this.CardList) {
            copy.CardList.Add(c.Clone());
        }

        return copy;
    }
}
