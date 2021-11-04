using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyManager : MonoBehaviour
{

    [SerializeField] private Transform dropZone;
    [SerializeField] private Transform playerZone;

    [SerializeField] private Transform nextPlayer1Zone;
    [SerializeField] private Transform nextPlayer2Zone;
    [SerializeField] private Transform nextPlayer3Zone;

    private static HierarchyManager instance;
    public static HierarchyManager Instance => instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void SetDropZoneAsParent(CardDisplay cardDisplay)
    {
        cardDisplay.transform.SetParent(dropZone, false);
    }

    public void SetPlayerZoneAsParent(CardDisplay cardDisplay)
    {
        cardDisplay.transform.SetParent(playerZone, false);
    }
    
    public void SetNextPlayerZoneAsParent(CardDisplay cardDisplay, int nextPlayer)
    {
        switch (nextPlayer)
        {
            case 1: cardDisplay.transform.SetParent(nextPlayer1Zone, false); break;
            case 2: cardDisplay.transform.SetParent(nextPlayer2Zone, false); break;
            case 3: cardDisplay.transform.SetParent(nextPlayer3Zone, false); break;
        }

    }

}
