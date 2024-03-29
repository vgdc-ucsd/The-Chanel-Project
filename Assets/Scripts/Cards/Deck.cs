using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    public List<UnitCard> CardList = new List<UnitCard>();

    private void addCard(UnitCard card)
    {
        CardList.Add(card);
    }
}
