using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

/*
THINGS TO DO:

        *add ready and start button
        *deal 7 cards if both players are ready
        *implement card logic and value
*/

// [Server] means only the server is permitted to call this method.
// [Command] means only client objects with authority can call this, and it only executes on the server. It is server code.
// [ClientRpc] means only the server can call this method, but it only runs on clients.


public class PlayerManager : NetworkBehaviour, IPlayerInterface
{
    // public bool IsMyTurn = false;
    bool skip = false;
    bool drew = false;
    bool playedWild;
    string playerName;

    List<CardDisplay> handList = new List<CardDisplay>();
    public GameManager gameManager;

    [SyncVar]
    int cardsPlayed = 0;

   	public bool skipStatus
   	{ //returns if the player should be skipped
   		get { return skip; }
   		set { skip = value; }
   	}

    public override void OnStartClient()
    {
        base.OnStartClient();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Debug.Log("Client ready");
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
                card.transform.SetParent(gameManager.PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(gameManager.Player3Area.transform, false);
                card.GetComponent<CardFlipper>().Flip();
            }
        } 
        else if (type == "Played")
        {
            card.transform.SetParent(gameManager.DropZone.transform, false);

            if(!hasAuthority)
            card.GetComponent<CardFlipper>().Flip();
        }
    }



    public void turn()
    { //does the turn
        playedWild = false;
        drew = false;
        int i = 0;
        foreach (CardDisplay x in handList)
        { //foreach card in hand

            GameObject temp = null;
            if (GameObject.Find("GameManager").GetComponent<GameManager>().PlayerArea.transform.childCount > i) //is the card already there or does it need to be loaded
                temp = GameObject.Find("GameManager").GetComponent<GameManager>().PlayerArea.transform.GetChild(i).gameObject;
            else
                temp = x.loadCard(GameObject.Find("GameManager").GetComponent<GameManager>().PlayerArea.transform);


           // if (handList[i].Equals(PlayerManager.discard[PlayerManager.discard.Count - 1]) || handList[i].getNumb() >= 13)
           // { //if the cards can be played
           //     SetListeners(i, temp);
           // }
           // else
           // {
           //     temp.transform.GetChild(3).gameObject.SetActive(true); //otherwise black them out
           // }
           // i++;
        }
    }

    public void addCards(CardDisplay other)
	{ //recieves cards to add to the hand
		handList.Add(other);
	}
    
    public bool Equals(IPlayerInterface other)
    { //equals function based on name
    	return other.getName().Equals(name);
    }
    public string getName()
    { //returns the name
    	return name;
    }
    public int getCardsLeft()
    { //gets how many cards are left in the hand
    	return handList.Count;
    }
}