using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DrawCards : MonoBehaviour
{
    [HideInInspector]
    public PlayerManager playerManager;


    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
        playerManager.CmdRequestToDrawCards();
       // Debug.Log(playerManager);
       // Debug.Log(networkIdentity.hasAuthority);
    }

}
