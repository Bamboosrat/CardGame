using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public UIManager UIManager;
    public int TurnOrder = 0;
    public string GameState = "Initialize {}";

    public List<IPlayerInterface> players = new List<IPlayerInterface>();

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

    public static List<CardDisplay> deck = new List<CardDisplay>();
    public static List<CardDisplay> discard = new List<CardDisplay>();

    private GameObject discardPileObj;

    public GameObject startButton;

    private int ReadyClicks = 0;
    public bool isGameStarting;

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
        

    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        GenerateDeck();
        startButton.SetActive(false);

    }

    private void Update()
    {
        if ( ReadyClicks == 1)
        {
            startButton.SetActive(true);
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
        Debug.Log(deck.Count);
    }

    public void ChangeGameState(string stateRequest)
    {
        if (stateRequest == "Initialize {}")
        {
            ReadyClicks = 0;
            GameState = "Initialize {}";
        }
        else if (stateRequest == "Compile {}")
        {
            if (ReadyClicks == 1)
            {
                GameState = "Compile {}";
                UIManager.HighlightTurn(TurnOrder);
            }
        }
        else if (stateRequest == "Execute {}")
        {
            GameState = "Execute {}";
            TurnOrder = 0;
        }
        UIManager.UpdateButtonText(GameState);
    }

    public void ChangeReadyClicks()
    {
        ReadyClicks++;
        Debug.Log(ReadyClicks);
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
        NetworkServer.Spawn(discardPileObj);

        deck.RemoveAt(0);

    }

    [Command]
    // All Command methods start with Cmd
    public void CmdDealCards()
    {
        // Debug.Log("Dealing " + handList.Count + " to " + connectionToClient);

        //addCards(deck[0]);
        GameObject temp = deck[0].loadCard(GameObject.Find("Main Canvas").transform);

        NetworkServer.Spawn(temp, connectionToClient);

       // RpcShowCard(temp, "Dealt");

        deck.RemoveAt(0);

        //  Debug.Log("A Client called a command, client's id is: " + connectionToClient);
    }


    public void DealStartHand()
    {
        Debug.Log("Deal starting Hand for each player");
        // TODO: Add for each player in game deal hand cards

        foreach (IPlayerInterface x in players)
        {
            for (int i = 0; i < 7; i++)
            {
                x.addCards(deck[0]);
                deck.RemoveAt(0);
            }
        }

    }
}
