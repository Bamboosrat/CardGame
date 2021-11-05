using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Lobby;

public class GameManager : NetworkBehaviour
{

    #region Field / Property
    private CardDeck cardDeck;
    private CardDeck discardDeck;

    public Card TopCard => discardDeck.PeekTopCard();

    [SyncVar]
    private int where = 0;

    private float timer = 0;
    private bool reverse = false;

    [SyncVar(hook = nameof(UpdateClientTurn))]
    public GameState gameState;


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

    [Server]
    void Awake()
    {

        Debug.Log("Start GameManager");

        cardDeck = new CardDeck();
        discardDeck = new CardDeck();

        cardDeck.GenerateDeck();

        DealFirstCard();

        Debug.Log(Room.GamePlayers.Count);
    }

    private void Update()
    {

        if (Room.GamePlayers[where].TurnStatus())
        {
              if (Room.GamePlayers[where].SkipStatus)
              {
                  Room.GamePlayers[where].SkipStatus = false;
                  where += reverse ? -1 : 1;
                  if (where >= Room.GamePlayers.Count)
                      where = 0;
                  else if (where < 0)
                      where = Room.GamePlayers.Count - 1;
                  return;
              }
            
              where += reverse ? -1 : 1;
              Room.GamePlayers[where + (reverse ? 1 : -1)].Turn();
                Debug.Log("AYAYA " + where);

        }

        if (where >= Room.GamePlayers.Count)
        {
            where = 0;
            Debug.Log(where);
        }
        else if (where < 0)
        {
            where = Room.GamePlayers.Count - 1;
            Debug.Log(where);
        }

        
        #region Turn Management Test
        /*
      // bool win = UpdateCardsLeft();
      // if (win)
      // return;

           if (Room.GamePlayers[where].TurnStatus())
           {
               if (Room.GamePlayers[where].SkipStatus)
               {
                   Room.GamePlayers[where].SkipStatus = false;
                   where += reverse ? -1 : 1;
                   if (where >= Room.GamePlayers.Count)
                       where = 0;
                   else if (where < 0)
                       where = Room.GamePlayers.Count - 1;
                   return;
               }
            
               where += reverse ? -1 : 1;
               Room.GamePlayers[where + (reverse ? 1 : -1)].Turn();
           }
           else if (Room.GamePlayers[where] != null)
           {
               if (Room.GamePlayers[where].SkipStatus)
               {
                   Room.GamePlayers[where].SkipStatus = false;
                   where += reverse ? -1 : 1;
                   if (where >= Room.GamePlayers.Count)
                       where = 0;
                   else if (where < 0)
                       where = Room.GamePlayers.Count - 1;
                   return;
               }
               timer += Time.deltaTime;
               if (timer < 2.2)
                   return;
               timer = 0;
               where += reverse ? -1 : 1;
               Room.GamePlayers[where + (reverse ? 1 : -1)].Turn();
           }
           else
               where += reverse ? -1 : 1;
       
           if (where >= Room.GamePlayers.Count)
               where = 0;
           else if (where < 0)
               where = Room.GamePlayers.Count - 1;
        */
        #endregion
    }

    #region Card Methods

    [Server]
   void DealFirstCard()
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
       // Debug.Log(playerM.getCardsLeft());
    }

    [Server]
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

            playerM.EndTurn();
            where += reverse ? -1 : 1;
        }

        

    }

    #endregion

    public void SpecialCardPlay(PlayerManager player, Card _card)
    { //takes care of all special cards played

        // Debug.Log("Special card played");
        
        int who = Room.GamePlayers.FindIndex(e => e.Equals(player)) + (reverse ? -1 : 1);
        if (who >= Room.GamePlayers.Count)
            who = 0;
        else if (who < 0)
            who = Room.GamePlayers.Count - 1;

        //Debug.Log(who);

        switch (_card.number)
        {
            // Skip
            case 10:
                Room.GamePlayers[who].SkipStatus = true;
                Debug.Log(who + " played (skip) " + _card.ToString());
                break;

                // reverse
            case 11:
                reverse = !reverse;
                int difference = 0;
                Debug.Log(who + " played (reverse) " + _card.ToString());
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
                Debug.Log(who + " played (draw2) " + _card.ToString());
                break;

                // draw 4
            case 14:
                DealCards(4, Room.GamePlayers[who]);
                Debug.Log(who + " played (wild4) " + _card.ToString());
                break;
        }
        // if (_card.number != 14)
        //    Debug.Log("Wild card played");
        //ColorChoicePanel.enabled = true;

        player.EndTurn();
        where += reverse ? -1 : 1;
    }

    public bool UpdateCardsLeft()
    { 

        foreach (PlayerManager playerM in Room.GamePlayers)
        {
            if (playerM.GetCardsLeft() == 0)
            {
                Debug.Log(playerM.GetName() + " won!");
               // endCan.SetActive(true);
               // endCan.transform.Find("WinnerTxt").gameObject.GetComponent<Text>().text = string.Format("{0} Won!", playerM.GetName());
                return true;
            }
        }
        return false;
    }

    private void UpdateClientTurn(GameState _oldGameState, GameState newGameState)
    {
        //  PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        //  player.IsMyTurn = player.MyGameState == gameState;
        //endTurnButton.interactable = player.IsMyTurn;
        //endTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = player.IsMyTurn ? "End Turn" : "Enemy Turn";
    }
}
