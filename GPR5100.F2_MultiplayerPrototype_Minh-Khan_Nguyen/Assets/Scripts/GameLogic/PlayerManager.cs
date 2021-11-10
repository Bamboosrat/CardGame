using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Lobby;
using UnityEngine.UI;

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

    [SerializeField]
    public GameObject gameOverScreen;
    [SerializeField]
    public GameObject chooseColor;
    [SerializeField]
    public GameObject UiPanel;

    private bool IsMyTurn = false;

    private bool isCardPlayable = false;
    private bool isSkipped = false;
    // private bool drew = false;
    // private bool playedWild;

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

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    private string displayName = "Loading...";



    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();


    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.GamePlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
        UpdateDisplay();
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
        Turn();

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
            if (true/*Room.GameManager.CanPlayCard(_card)*/)
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

    [Server]
    public void Turn()
    {
        IsMyTurn = true;
      //  Debug.Log("Start turn");
    }

    [Server]
    public void EndTurn()
    { 
        IsMyTurn = false;
        Room.GameManager.UpdateClientTurn();
       //Debug.Log("End turn");
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
    public bool IsCardPlayable() => isCardPlayable;

    public bool SkipStatus
    { //returns if the player should be skipped
        get { return isSkipped; }
        set { isSkipped = value; }
    }

    public bool TurnStatus() => IsMyTurn;

    public bool Equals(PlayerManager other)=> other.GetName().Equals(displayName);

    public string GetName() => displayName;

    public int GetCardsLeft() => handList.Count;

    public CardDisplay GetLastCard() => handList[handList.Count - 1];

    #endregion
}