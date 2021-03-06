using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;
using Lobby;
using TMPro;

public class GameManager : NetworkBehaviour
{
    #region Field / Property
    private CardDeck cardDeck;
    private CardDeck discardDeck;



    public Card TopCard => discardDeck.PeekTopCard();

    [SyncVar]
    private int where;

    //private float timer = 0;
    private bool reverse = false;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    #endregion

    #region Unity
    [Server]
    void Awake()
    {
        Debug.Log("Start GameManager");



        cardDeck = new CardDeck();
        discardDeck = new CardDeck();

        cardDeck.GenerateDeck();

        DealFirstCard();

        where = Room.GamePlayers.Count - 1;
    }

    #endregion

    #region Card Methods

    [Server]
   private void DealFirstCard()
   {
       Debug.Log("Starting Game. Dealing first Card");
       // Spawns first card
       List<Card> tempCardList = new List<Card>();

        while (cardDeck.PeekTopCard().number >= 10)
        {
            tempCardList.Add(cardDeck.GetTopCard());
        }
        Debug.Log("Set First Card: " + cardDeck.PeekTopCard().ToString());
        discardDeck.AddCard(cardDeck.GetTopCard());


        if (tempCardList.Count != 0) {
            foreach (Card cards in tempCardList)
            {
                cardDeck.AddCard(cards);
            }
        }

        cardDeck.Shuffle();

        GetComponent<CardDisplaySpawner>().SpawnCard(discardDeck.PeekTopCard(), CardDisplay.CardPosition.First, 0);

   }

    [Server]
    public bool CanPlayCard(Card card)
    {
       // Debug.Log("Try to play Card: " + card.ToString());
       // Debug.Log("Top Card: " + TopCard.ToString());
        return TopCard.IsPlayable(card);
    }

    [Server]
    public void PlayCard(Card card)
    {
        discardDeck.AddCard(card);
    }

    [Server]
    public void DealStartHand(PlayerManager playerM)
    {
       // Debug.Log("Deals start hand : "+ playerM);
       // Debug.Log("Deals start hand : " + cardDeck);

        for (int j = 0; j < 7; j++)
            {
            playerM.AddCard(cardDeck.GetTopCard());

            }

    }


    public void DealCards(int amountOfCards, PlayerManager playerM)
    {
        if (cardDeck.IsDeckEmpty())
        {
            cardDeck.ResetDeck(discardDeck);
        }
        else
        {

            for (int j = 0; j < amountOfCards; j++)
            {
                playerM.AddCard(cardDeck.GetTopCard());
                //Debug.Log(playerM);
            }


           // if (!TopCard.IsPlayable(playerM.GetLastCard().Card))
           // implement is drawn card playable? play it or end turn
            playerM.EndTurn();


        }
    }

    [Server]
    public void SpecialCardPlay(PlayerManager player, Card _card)
    { //takes care of all special cards played

        int who = Room.GamePlayers.FindIndex(e => e.Equals(player)) + (reverse ? -1 : 1);
        if (who >= Room.GamePlayers.Count)
            who = 0;
        else if (who < 0)
            who = Room.GamePlayers.Count - 1;

        switch (_card.number)
        {
            // Skip
            case 10:
                Room.GamePlayers[who].SkipStatus = true;
                Debug.Log(Room.GamePlayers[where].GetName() + " played (skip) " + _card.ToString());
                Debug.Log(Room.GamePlayers[who].GetName() + " is skipped. Skip status: " + Room.GamePlayers[who].SkipStatus);
                break;

            // reverse
            case 11:
                reverse = !reverse;
                int difference = 0;
                Debug.Log(Room.GamePlayers[where].GetName() + " played (reverse) " + _card.ToString());
                if (reverse)
                {
                    difference = who - 2;
                    if (difference >= 0)
                        where = difference;
                    else
                    {
                        difference = Mathf.Abs(difference);
                        where = Room.GamePlayers.Count - difference;
                    }
                }
                else
                {
                    difference = who + 2;
                    if (difference > Room.GamePlayers.Count - 1)
                        where = difference - Room.GamePlayers.Count;
                    else
                        where = difference;
                }
                break;

            // draw 2
            case 12:
                DealCards(2, Room.GamePlayers[who]);
                Debug.Log(Room.GamePlayers[where].GetName() + " played (draw2) " + _card.ToString());
                break;

            // draw 4
            case 14:
                DealCards(4, Room.GamePlayers[who]);
                Debug.Log(Room.GamePlayers[where].GetName() + " played (wild4) " + _card.ToString());
                break;
        }
        // if (_card.number != 14)
        //    Debug.Log("Wild card played");
        //ColorChoicePanel.enabled = true;

        player.EndTurn();
    }

    #endregion

    #region Update Client Turn

    private bool UpdateCardsLeft()
    {
        if (Room.GamePlayers[where].GetCardsLeft() == 0) {

               Room.GamePlayers[where].gameOverScreen.SetActive(true);
               Room.GamePlayers[where].gameOverScreen.transform.Find("Text_Winner").gameObject.GetComponent<Text>().text = string.Format("{0} Won!", Room.GamePlayers[where].GetName());
            
            return true;

        }
        
        return false;
    }

    [Server]
    public void UpdateClientTurn()
    {
        if (UpdateCardsLeft())
            return;

        Debug.Log(Room.GamePlayers[where].GetName() + " ended his turn. Player " + where);



        where += reverse ? -1 : 1;


        if (where >= Room.GamePlayers.Count)
            where = 0;
        else if (where < 0)
            where = Room.GamePlayers.Count - 1;



        if (Room.GamePlayers[where].SkipStatus)
        {
            // Debug.Log(Room.GamePlayers[where].GetName() + " is skipped");
            Room.GamePlayers[where].SkipStatus = false;
            where += reverse ? -1 : 1;
            if (where >= Room.GamePlayers.Count)
                where = 0;
            else if (where < 0)
                where = Room.GamePlayers.Count - 1;

            Room.GamePlayers[where].Turn();

            Debug.Log(Room.GamePlayers[where].GetName() + " his Turn started. Player " + where);
            return;
        }


        Room.GamePlayers[where].Turn();

        Debug.Log(Room.GamePlayers[where].GetName() + " his Turn started. Player " + where);
    }
    #endregion
}
