using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameManager gameManager;

    public void OnClick()
    {
        gameManager.isGameStarting = true;
        gameManager.DealStartHand();
        Debug.Log("Start Game");
        gameObject.SetActive(false);
    }
}
