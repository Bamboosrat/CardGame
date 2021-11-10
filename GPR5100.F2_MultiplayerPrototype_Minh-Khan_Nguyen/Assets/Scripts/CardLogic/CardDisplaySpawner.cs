using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardDisplaySpawner : MonoBehaviour
{

     [SerializeField]
     private CardDisplay regCardPrefab;
     [SerializeField]
     private CardDisplay skipCardPrefab;
     [SerializeField]
     private CardDisplay drawCardPrefab;
     [SerializeField]
     private CardDisplay wildCardPrefab;
     [SerializeField]
     private CardDisplay reverseCardPrefab;

    [Server]
    public CardDisplay SpawnCard(Card _card, CardDisplay.CardPosition position, int playerID, NetworkConnection conn = null)
    {
        CardDisplay cardTemp = null;
        // Debug.Log("Spawn card");
        switch (_card.number)
        {
            case int n when (n < 10):
                cardTemp = Instantiate(regCardPrefab);
                break;

            case 10:
                cardTemp = Instantiate(skipCardPrefab);
                break;

            case 11:
                cardTemp = Instantiate(reverseCardPrefab);
                break;

            case 12:
                cardTemp = Instantiate(drawCardPrefab);
                break;

            case 13:
                cardTemp = Instantiate(wildCardPrefab);
                break;

            case 14:
                cardTemp = Instantiate(wildCardPrefab);
                break;

        }
        cardTemp.cardPosition = position;
        cardTemp.SetCardProperty(_card);
        cardTemp.ownerID = playerID;

        // Debug.Log("Spawn Card: " + _card.ToString());

        NetworkServer.Spawn(cardTemp.gameObject, conn);

        
        return cardTemp;
    }

}
