using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
<<<<<<< Updated upstream
    public List<Card> CardList;
=======
    public List<UnitCard> CardList = new List<UnitCard>();

    private void addCard(UnitCard card)
    {
        CardList.Add(card);
    }
>>>>>>> Stashed changes
}
