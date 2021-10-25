using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Lobby;

public class GameManager : NetworkBehaviour
{
    
    public GameObject PlayerArea;
    public GameObject Player2Area;
    public GameObject Player3Area;
    public GameObject Player4Area;
    public GameObject DropZone;

    public GameObject regCardPrefab;
    public GameObject skipCardPrefab;
    public GameObject drawCardPrefab;
    public GameObject wildCardPrefab;
    public GameObject reverseCardPrefab;

    public  List<CardDisplay> deck = new List<CardDisplay>();
    public  List<CardDisplay> discard = new List<CardDisplay>();

    private GameObject discardPileObj;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    void Start()
    {
       // UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
      //  UIManager.UpdateButtonText(GameState);

        discard.Clear();
        deck.Clear();

        PlayerArea = GameObject.Find("PlayerArea");
        Player2Area = GameObject.Find("Player2Area");
        Player3Area = GameObject.Find("Player3Area");
        Player4Area = GameObject.Find("Player4Area");
        DropZone = GameObject.Find("DropZone");

        GenerateDeck();

        // cant find GameManager in scene when loaded so gotta backward logic this shit
        foreach (var item in Room.GamePlayers)
        {
            item.GetComponent<PlayerManager>().gameManager = this.gameObject.GetComponent<GameManager>();
        }

    }

    void GenerateDeck()
    {
        Debug.Log("Generating Deck");

        // Generate deck
        for (int i = 0; i < 15; i++)
        { //setups the deck by making cards
            for (int j = 0; j < 8; j++)
            {
                switch (i)
                {
                    case 10:
                        deck.Add(new CardDisplay(i, ReturnColorName(j % 4), regCardPrefab));
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
        DealFirstCard();
        //DealStartHand();
        Debug.Log(deck.Count);
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
    {
       // Debug.Log("Shuffling Deck");
        //shuffles the deck by changing cards around
        for (int i = 0; i < deck.Count; i++)
        {
            CardDisplay temp = deck.ElementAt(i);
            int posSwitch = Random.Range(0, deck.Count);
            deck[i] = deck[posSwitch];
            deck[posSwitch] = temp;
        }
    }

    void DealFirstCard()
    {
        Debug.Log("Starting Game. Dealing first Card");
        // Spawns first card
        CardDisplay first = null;

        // Number Card
        if (deck[0].getNumb() < 10)
        {
            first = deck[0];
        }
        else
        {
            // Special Card
            while (deck[0].getNumb() >= 10)
            {
                deck.Add(deck[0]);
                deck.RemoveAt(0);
            }
            first = deck[0];
        }

        // Put the first card in the deck in the discard pile
        discard.Add(first);
        discardPileObj = first.loadCard(GameObject.Find("DropZone").transform);
        NetworkServer.Spawn(discardPileObj, connectionToClient);

        deck.RemoveAt(0);
    }

    

    // All Rpc methods start with Rpc
    [ClientRpc]
    public void RpcShowCard(GameObject card, string type)
    {

        if (type == "Dealt")
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

            if (!hasAuthority)
                card.GetComponent<CardFlipper>().Flip();
        }
    }


    public void DealStartHand()
    {
        Debug.Log("Deal starting Hand for each player");
        // TODO: Add for each player in game deal hand cards
        
       foreach (NetworkGamePlayerLobby x in Room.GamePlayers)
       {
            PlayerManager temp = x.GetComponent<PlayerManager>();
           for (int i = 0; i < 7; i++)
           {
               temp.addCards(deck[0]);
               deck.RemoveAt(0);
           }
       }

    }
}
