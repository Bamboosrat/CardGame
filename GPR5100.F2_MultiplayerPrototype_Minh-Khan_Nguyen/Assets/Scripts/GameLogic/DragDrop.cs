using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragDrop : NetworkBehaviour
{
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private bool isDraggable = true;

    public GameObject Canvas;
    public GameObject DropZone;
    public PlayerManager playerManager;

    private GameObject StartParent;
    private GameObject dropZone;

    private Vector3 startPosition;


    private void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
        DropZone = GameObject.Find("DropZone");

        if (!hasAuthority)
            isDraggable = false;
    }


    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            transform.SetParent(Canvas.transform, true);
        }
     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }

    public void StartDrag()
    {
        if (!isDraggable) return;
        StartParent = transform.parent.gameObject;
        startPosition = transform.position;
        gameObject.transform.localScale = transform.localScale * 2f;
        isDragging = true;
    }

    public void EndDrag()
    {
        if (!isDraggable) return;
        gameObject.transform.localScale = transform.localScale / 2f;
        isDragging = false;
        if (isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);
            isDraggable = false;
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            playerManager = networkIdentity.GetComponent<PlayerManager>();
            playerManager.PlayCard(gameObject);
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(StartParent.transform, false);
        }
    }
}
