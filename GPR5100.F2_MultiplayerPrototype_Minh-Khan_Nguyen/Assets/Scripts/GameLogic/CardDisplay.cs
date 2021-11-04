using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Lobby;

/*
	* 1-9 are regular
	* 10 is skip
	* 11 is reverse
	* 12 is draw 2
	* 13 is wild
	* 14 is wild draw 4
	*/

public class CardDisplay : NetworkBehaviour
{

    #region Field / Property
    [SyncVar]
	private Card card;
	public Card Card => card;

	[SyncVar]
	public CardPosition cardPosition;

	/// <summary>
	/// OwnerID is the same as PlayerID on PlayerManager
	/// OwnerID 0 stands for already played or first card
	/// </summary>
	[SyncVar]
	public int ownerID;

	public enum CardPosition
    {
		Played,
		Dealt,
		First
    }


	private NetworkManagerLobby room;
	private NetworkManagerLobby Room
	{
		get
		{
			if (room != null) { return room; }
			return room = NetworkManager.singleton as NetworkManagerLobby;
		}
	}

    #endregion

    [Server]
	public void SetCardProperty(Card _card)
    {
		this.card = _card;

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

		// implement 
		/*if(hasAuthority || cardPosition != CardPosition.Dealt)
        {
			if(isClientOnly)
				CmdRequestCard();
			else
				LoadCardDisplay();
        }*/

		LoadCardDisplay();

		// Debug.Log("CardPosition onStartClient: " + cardPosition);

		if (cardPosition == CardPosition.Dealt)
		{
			int playerID = PlayerManager.localPlayerManager.PlayerID;
			if (playerID == ownerID)
				HierarchyManager.Instance.SetPlayerZoneAsParent(this);
            else
            {
				if (Room.GamePlayers.Count == 2) 
				{
					HierarchyManager.Instance.SetNextPlayerZoneAsParent(this, 2);
				}
				else
				{
					int playerZoneID = ((ownerID - playerID) + 4) % 4;
					HierarchyManager.Instance.SetNextPlayerZoneAsParent(this, playerZoneID);
				}
			}
		}
		else
		{
			HierarchyManager.Instance.SetDropZoneAsParent(this);
		}
	
	}

    #region LoadCard

    [Client]
	private void LoadCardDisplay()
    {
		name = card.Color + card.Number.ToString();

		switch (card.number)
		{
			case int n when (n < 10):
				foreach (Transform childs in transform)
				{
					if (childs.name.Equals("Cover") || childs.name.Equals("CardBack"))
						break;

					childs.GetComponent<Text>().text = card.Number.ToString();

				}
				transform.GetChild(1).GetComponent<Text>().color = ReturnColor(card.Color);
				break;

				// skip, reverse draw 2
			case int n when (n >= 10 && n <= 12):
				transform.GetChild(1).GetComponent<Image>().color = ReturnColor(card.Color);
				break;

				// special black wild
			case 13:
				transform.GetChild(0).GetComponent<Text>().text = "";
				transform.GetChild(2).GetComponent<Text>().text = "";
				break;
		}

		GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/" + card.Color + "Card");
		ShowCard(this, cardPosition);
	}

	private Color ReturnColor(Card.CardColor what)
	{ //returns a color based on the color string
		switch (what)
		{
			case Card.CardColor.green:
				return new Color32(0x55, 0xaa, 0x55, 255);
			case Card.CardColor.blue:
				return new Color32(0x55, 0x55, 0xfd, 255);
			case Card.CardColor.red:
				return new Color32(0xff, 0x55, 0x55, 255);
			case Card.CardColor.yellow:
				return new Color32(0xff, 0xaa, 0x00, 255);
		}
		return new Color(0, 0, 0);
	}

    #endregion

    public void ShowCard(CardDisplay _card, CardPosition type)
    {
			//Debug.Log("Card authority: " + hasAuthority);
		if (!hasAuthority && type == CardPosition.Dealt)
		{
			//Debug.Log(type);
			//Debug.Log("Flip card");
            _card.GetComponent<CardFlipper>().Flip();
			
        }
    }
}
