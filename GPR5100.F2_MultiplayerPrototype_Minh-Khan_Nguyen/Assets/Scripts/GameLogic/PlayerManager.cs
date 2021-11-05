using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Lobby;

/*
THINGS TO DO:

        * Implement special card logic
        * Especially wild card logic
        * Turn based
        * Game over
        * Option menu
        * Display name + correct color board
        
        
        
        Bug list: 
        * Fix BUG in List.Find() method, if a card with the same value and color gets played, the other gets "played" too
        
*/

public class PlayerManager : NetworkBehaviour, IPlayerInterface
{
    #region Field / Property

    //[SyncVar]
    private bool IsMyTurn = false;


    private bool isCardPlayable = false;
    private bool skip = false;
    // private bool drew = false;
    // private bool playedWild;

    public GameState MyGameState;

    private List<CardDisplay> handList = new List<CardDisplay>();

    // [SyncVar]
    // int cardsPlayed = 0;
    // int cardsDrawn = 0;

    public static PlayerManager localPlayerManager { get; private set; }

    [SyncVar]
    private int playerID;
    public int PlayerID => playerID;

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

    #region LobbyStuff

    [SyncVar]
    private string displayName = "Loading...";

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);


    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Server]
    public void SetPlayerID(int ID)
    {
        playerID = ID;
    }

    #endregion

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayerManager = this;
        CmdRequestStartHand();
    }



    [Command]
    private void CmdRequestStartHand()
    {
        Room.GameManager.DealStartHand(this);
    }

    #region PlayCard

    public void PlayCard(CardDisplay _card)
    {
        CmdPlayCard(_card.Card);
        // cardsPlayed++;
        //Debug.Log(cardsPlayed);
    }

    [Command]
    void CmdPlayCard(Card _card)
    {
        isCardPlayable = false;

        if (IsMyTurn)
        {
            //Debug.Log("Card Played");
            if (Room.GameManager.CanPlayCard(_card))
            {
                isCardPlayable = true;
                Room.GameManager.PlayCard(_card);
                //Debug.Log("Player " + playerID  + " " + _card.ToString());
                RemoveCard(_card);
                Room.CardDisplaySpawner.SpawnCard(_card, CardDisplay.CardPosition.Played, 0);
                Room.GameManager.SpecialCardPlay(this, _card);

               // Debug.Log(IsMyTurn);
            }
            else
            {
                TargetRPCCantPlayCard();
            }
        }
        else
        {
            Debug.Log("It is not my turn");
        }
    }

    [TargetRpc]
    private void TargetRPCCantPlayCard()
    {
        Debug.Log("Not Allowed to play Card");
        isCardPlayable = false;
    }



    #endregion

    [Server]
    private CardDisplay FindCard(Card card)
    {
        return handList.Find(cardInHandList => cardInHandList.Card.Number == card.Number && cardInHandList.Card.Color == card.Color);
    }

    [Command]
    public void CmdRequestToDrawCards()
    {
        if (IsMyTurn)
        {
            Room.GameManager.DealCards(1, this);
            // EndTurn();
        }    
    }

    public void Turn()
    {
        IsMyTurn = true;
    }

    public void EndTurn()
    { //ends the player's turn
        IsMyTurn = false;

    }



    #region Add or Remove Method
    [Server]
    public void AddCard(Card other)
	{ //receives cards to add to the hand
        CardDisplay cardDisplay = Room.CardDisplaySpawner.SpawnCard(other, CardDisplay.CardPosition.Dealt, PlayerID, connectionToClient);
        //bool succesfulTest = cardDisplay.netIdentity.AssignClientAuthority(connectionToClient);
        //Debug.Log("Assign Client Authority: " + succesfulTest);
		handList.Add(cardDisplay);
	}

    [Server]
    public void RemoveCard(Card other)
    {
        CardDisplay cardDisplay = FindCard(other);
        //Debug.Log("Remove Card");
        if(cardDisplay != null)
        {
            handList.Remove(cardDisplay);
            NetworkServer.Destroy(cardDisplay.gameObject);
        }
    }
    #endregion



    #region Get Information
    public bool IsCardPlayable()
    {
        return isCardPlayable;
    }

    public bool SkipStatus
    { //returns if the player should be skipped
        get { return skip; }
        set { skip = value; }
    }

    public bool TurnStatus() => IsMyTurn;

    public bool Equals(PlayerManager other)
    { //equals function based on name
    	return other.GetName().Equals(displayName);
    }
    public string GetName()
    { //returns the name
    	return displayName;
    }
    public int GetCardsLeft()
    { //gets how many cards are left in the hand
    	return handList.Count;
    }
    #endregion
}