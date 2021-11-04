using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Lobby;

public class GameManager : NetworkBehaviour
{
    private CardDeck cardDeck;
    private CardDeck discardDeck;

    public Card TopCard => discardDeck.PeekTopCard();

    private int where = 0;
   // private float timer = 0;
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

    [Server]
    void Awake()
    {

        Debug.Log("Start GameManager");

        cardDeck = new CardDeck();
        discardDeck = new CardDeck();

        cardDeck.GenerateDeck();

        DealFirstCard();
    }

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
        }

    }

    private void UpdateClientTurn(GameState _oldGameState, GameState newGameState)
    {
        PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        player.IsMyTurn = player.MyGameState == gameState;
        //endTurnButton.interactable = player.IsMyTurn;
        //endTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = player.IsMyTurn ? "End Turn" : "Enemy Turn";
    }

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
                break;

                // reverse
            case 11:
                reverse = !reverse;
                int difference = 0;
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
                break;

                // draw 4
            case 14:
                DealCards(4, Room.GamePlayers[who]);
                break;
        }
      // if (_card.number != 14)
      //     ColorChoicePanel.enabled = true;
    }
}
