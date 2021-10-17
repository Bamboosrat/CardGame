using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
THINGS TO DO:

        *add ready and start button
        *deal 7 cards if both players are ready
        *implement card logic and value
        



*/




public class PlayerManager : NetworkBehaviour
{
    public enum gameState
    {
        none,
        startGame,
        inGame,
        endGame
    }

    private gameState currentState = gameState.none;


    public GameObject Card1;
    public GameObject Card2;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject DropZone;

    List<GameObject> cards = new List<GameObject>();


    [SyncVar]
    int cardsPlayed = 0;


    public override void OnStartClient()
    {
        base.OnStartClient();


        currentState = gameState.startGame;

        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        DropZone = GameObject.Find("DropZone");
    }

    [Server]
    public override void OnStartServer()
    {
        cards.Add(Card1);
        cards.Add(Card2);
    }


    // Command is a request from client -> Server to run a method/ function
    // All Command methods start with Cmd
    [Command]
    public void CmdDealCards()
    {
        if (currentState == gameState.startGame)
        {
            DrawCards(7);

            currentState = gameState.inGame;
        } 
        else if( currentState == gameState.inGame)
        {

            DrawCards(1);

        }
    }

    public void DrawCards(int numberOfCards)
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject card = Instantiate(cards[Random.Range(0, cards.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");
        }
    }
    

   
    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
        cardsPlayed++;
        Debug.Log(cardsPlayed);
    }

    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");
    }

    // All Rpc methods start with Rpc
    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if(type == "Dealt")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
                card.GetComponent<CardFlipper>().Flip();
            }
        } 
        else if (type == "Played")
        {
            card.transform.SetParent(DropZone.transform, false);

            if(!hasAuthority)
            card.GetComponent<CardFlipper>().Flip();
        }
    }


    
}
