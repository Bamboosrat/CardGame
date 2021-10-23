using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ReadyUp : NetworkBehaviour
{
    public PlayerManager playerManager;

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
        playerManager.gameManager.ChangeReadyClicks();
        playerManager.gameManager.players.Add(playerManager);
        Debug.Log("Amount of Players: " + playerManager.gameManager.players.Count);
        gameObject.SetActive(false);
    }
}
