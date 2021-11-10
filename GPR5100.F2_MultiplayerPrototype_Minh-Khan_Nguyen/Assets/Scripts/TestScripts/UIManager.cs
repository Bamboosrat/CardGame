using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject player1NameTag;
    [SerializeField] private GameObject player2NameTag;
    [SerializeField] private GameObject player3NameTag;
    [SerializeField] private GameObject player4NameTag;

    private static UiManager instance;
    public static UiManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

}
