using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class CardDeck 
{
    private Stack<Card> deck = new Stack<Card>();

    private const int amountOfBlackCards = 10;

    public void GenerateDeck()
    {
        Debug.Log("Generating Deck");

        // loop over all 4 colors
        for (int c = 0; c < 4; c++)
        {
            for (int n = 0; n < 13; n++)
            {
                deck.Push(new Card(n, c));
            }
        }

        for (int i = 0; i < amountOfBlackCards; i++)
        {
            deck.Push(new Card(13+(i%2), Card.CardColor.black));
        }

        Shuffle();

        //Debug.Log(deck[0].getNumb());
    }

    public void Shuffle()
    {
        List<Card> list = deck.ToList();
        
        // Debug.Log("Shuffling Deck");
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck.ElementAt(i);
            int posSwitch = Random.Range(0, deck.Count);
            list[i] = list[posSwitch];
            list[posSwitch] = temp;
            
        }

        deck.Clear();

        foreach (Card card in list)
        {
            deck.Push(card);
        }
    }

    // removes top card from stack "deck"
    public Card GetTopCard() => deck.Pop();

    // adds card to top of stack "deck"
    public void AddCard(Card card) => deck.Push(card);

    public Card PeekTopCard() => deck.Peek();

    public bool IsDeckEmpty()
    {
        if (deck.Count == 0)
            return true;
        else
            return false;
    }

    public int CountCards() => deck.Count();

    public void ResetDeck(CardDeck otherDeck)
    {
        for (int i = 0; i < otherDeck.CountCards() - 1; i++)
        {
            AddCard(otherDeck.GetTopCard());
        }

        Shuffle();
    }

}
