using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragDrop : NetworkBehaviour
{
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private bool isDraggable = true;

    private GameObject Canvas;

    private PlayerManager playerManager;

    private GameObject StartParent;
    private GameObject dropZone;

    private Vector3 startPosition;


    private void Start()
    {
        Canvas = GameObject.Find("Main Canvas");

        NetworkIdentity netWorkIdentity = NetworkClient.connection.identity;
        playerManager = netWorkIdentity.GetComponent<PlayerManager>();

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
           playerManager.PlayCard(gameObject.GetComponent<CardDisplay>());
                
            if (!playerManager.IsCardPlayable())
            {
                transform.position = startPosition;
                transform.SetParent(StartParent.transform, false);
            }
            else
            {
                isDraggable = false;
            }
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(StartParent.transform, false);
        }
        
        
    }
}
