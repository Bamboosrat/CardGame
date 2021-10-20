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


public class PlayerManager : NetworkBehaviour
{
    public enum GameState
    {
        none,
        startGame,
        inGame,
        endGame
    }

    List<IPlayerInterface> players = new List<IPlayerInterface>();

    private GameState currentState = GameState.none;

    public GameObject regCardPrefab;
    public GameObject skipCardPrefab;
    public GameObject drawCardPrefab;
    public GameObject wildCardPrefab;
    public GameObject reverseCardPrefab;

    public GameObject PlayerArea;
    public GameObject Player2Area;
    public GameObject Player3Area;
    public GameObject Player4Area;
    public GameObject DropZone;

    public static GameObject discardPileObj;

    public List<GameObject> cards = new List<GameObject>();

    public static List<CardDisplay> deck = new List<CardDisplay>();
    public static List<CardDisplay> discard = new List<CardDisplay>();

    public List<CardDisplay> handList = new List<CardDisplay>();

    [SyncVar]
    int cardsPlayed = 0;


    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        Player2Area = GameObject.Find("Player2Area");
        Player3Area = GameObject.Find("Player3Area");
        Player4Area = GameObject.Find("Player4Area");
        DropZone = GameObject.Find("DropZone");

        Debug.Log("Client ready");
    }

    [Server]
    public override void OnStartServer()
    {
        discard.Clear();
        deck.Clear();
        handList.Clear();

        

        //cards.Add(regCardPrefab);

        currentState = GameState.startGame;

        for (int i = 0; i < 15; i++)
        { //setups the deck by making cards
            for (int j = 0; j < 8; j++)
            {
                switch (i)
                {
                    case 10:
                        deck.Add(new CardDisplay(i, ReturnColorName(j % 4), skipCardPrefab));
                        break;
                    case 11:
                        deck.Add(new CardDisplay(i, ReturnColorName(j % 4), reverseCardPrefab));
                        break;
                    case 12:
                        deck.Add(new CardDisplay(i, ReturnColorName(j % 4), drawCardPrefab));
                        break;
                    case 13:
                        deck.Add(new CardDisplay(i, "Black", wildCardPrefab));
                        break;
                    case 14:
                        deck.Add(new CardDisplay(i, "Black", wildCardPrefab));
                        break;
                    default:
                        deck.Add(new CardDisplay(i, ReturnColorName(j % 4), regCardPrefab));
                        break;
                }
                
                if ((i == 0 || i >= 13) && j >= 3)
                    break;
            }
        }
        Shuffle();


        // Spawns first card
        CardDisplay first = null;
        if (deck[0].getNumb() < 10)
        {
            first = deck[0];
        }
        else
        {
            while (deck[0].getNumb() >= 10)
            {
                deck.Add(deck[0]);
                deck.RemoveAt(0);
            }
            first = deck[0];
        }
        discard.Add(first);
        discardPileObj = first.loadCard(GameObject.Find("DropZone").transform);
        deck.RemoveAt(0);

        // Add for each player in game deal hand cards
        
            for (int i = 0; i < 7; i++)
            {
                handList.Add(deck[0]);
                deck.RemoveAt(0);

            Debug.Log("Cards in Hand: " + handList.Count);
            }


        Debug.Log("Server start");

    }

    // All Command methods start with Cmd
    [Command]
    public void CmdDealCards()
    {

        DrawCards(7);


    }

    public void DrawCards(int numberOfCards)
    {
        for (int i = 0; i < handList.Count; i++)
        {
            GameObject temp = handList[i].loadCard(GameObject.Find("Main Canvas").transform);//Instantiate(cards[Random.Range(0, cards.Count)], new Vector2(0, 0), Quaternion.identity);
            Debug.Log(i);
            NetworkServer.Spawn(temp, connectionToClient);
            RpcShowCard(temp, "Dealt");
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
                card.transform.SetParent(Player3Area.transform, false);
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


    string ReturnColorName(int numb)
    { //returns a color based on a number, used in setup
        switch (numb)
        {
            case 0:
                return "Green";
            case 1:
                return "Blue";
            case 2:
                return "Red";
            case 3:
                return "Yellow";
        }
        return "";
    }

    void Shuffle()
    { //shuffles the deck by changing cards around
        for (int i = 0; i < deck.Count; i++)
        {
            CardDisplay temp = deck.ElementAt(i);
            int posSwitch = Random.Range(0, deck.Count);
            deck[i] = deck[posSwitch];
            deck[posSwitch] = temp;
        }
    }
}
