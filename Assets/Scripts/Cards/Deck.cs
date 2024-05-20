using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DrawStatus
{
    Available, Discard, InPlay
}


// NOTE: Don't use the same deck for two teams, create a new deck with same contents instead.

[CreateAssetMenu(fileName = "New Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    public List<Card> CardList = new List<Card>();
    [HideInInspector] public int numAvailableCards = 0;

    private void addCard(UnitCard card)
    {
        CardList.Add(card);
    }

    private void Awake()
    {

    }
    public void LoadCards(List<Card> cards)
    {
        CardList = cards;
    }

    public void Init()
    {
        List<Card> freshCardList = new List<Card>();
        foreach (Card card in CardList)
        {
            freshCardList.Add(ScriptableObject.Instantiate(card));
        }
        CardList = freshCardList;
        numAvailableCards = 0;
        for (int i = 0; i < CardList.Count; i++)
        {
            CardList[i].id = i;
            CardList[i].drawStatus = DrawStatus.Available;
            numAvailableCards++;
        }
    }

    public Deck Clone() {
        Deck copy = ScriptableObject.Instantiate(this);
        copy.CardList = new List<Card>();
        foreach (Card c in this.CardList) {
            //Debug.Log(c.name);

            copy.CardList.Add(c.Clone());
            //Debug.Log(uc.name);
        }

        copy.numAvailableCards = numAvailableCards;

        return copy;
    }

    public Card RandomAvailableCard()
    {
        List<Card> availableCards = new List<Card>();
        foreach (Card c in CardList)
        {
            if (c.drawStatus == DrawStatus.Available)
            {
                availableCards.Add(c);
            }
        }
        if (availableCards.Count == 0) return null;

        return availableCards[Random.Range(0, availableCards.Count)];
    }

    public void Discard(Card card)
    {
        foreach (Card c in CardList)
        {
            if (card.id == c.id)
            {
                c.drawStatus = DrawStatus.Discard;
            }
        }
    }

    public void Refresh()
    {
        foreach (Card c in CardList)
        {
            if (c.drawStatus == DrawStatus.Available)
                Debug.LogError("Tried to refresh deck while draw pile is not empty");
            if (c.drawStatus == DrawStatus.Discard)
            {
                c.drawStatus = DrawStatus.Available;
                numAvailableCards++;
            }
        }
    }

    public bool DrawPileIsEmpty()
    {
        return (numAvailableCards == 0);


        // bad code
        /*
        foreach (Card c in CardList)
        {
            if (c.drawStatus == DrawStatus.Available) return false;
        }
        return true;
        */
    }

    public List<Card> DiscardPile()
    {
        List<Card> cards = new List<Card>();
        foreach (Card card in CardList)
        {
            if (card.drawStatus == DrawStatus.Discard) cards.Add(card);
        }
        return cards;
    }

    public List<Card> DrawPile()
    {
        List<Card> cards = new List<Card>();
        foreach (Card card in CardList)
        {
            if (card.drawStatus == DrawStatus.Available) cards.Add(card);
        }
        return cards;
    }
}
