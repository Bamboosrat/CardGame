using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : MonoBehaviour, IPlayerInterface
{

	bool skip = false;
	bool drew = false;
	bool playedWild;
	string playerName;
	List<CardDisplay> handList = new List<CardDisplay>();

	public HumanPlayer(string name)
	{ //initalizes
		playerName = name;
	}

	public bool skipStatus
	{ //returns if the player should be skipped
		get { return skip; }
		set { skip = value; }
	}

	public void turn()
	{ //does the turn
		playedWild = false;
		drew = false;
		int i = 0;
		foreach (CardDisplay x in handList)
		{ //foreach card in hand

			GameObject temp = null;
			if (GameObject.Find("PlayerManager").GetComponent<PlayerManager>().PlayerArea.transform.childCount > i) //is the card already there or does it need to be loaded
				temp = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().PlayerArea.transform.GetChild(i).gameObject;
			else
				temp = x.loadCard(GameObject.Find("PlayerManager").GetComponent<PlayerManager>().PlayerArea.transform);


			if (handList[i].Equals(PlayerManager.discard[PlayerManager.discard.Count - 1]) || handList[i].getNumb() >= 13)
			{ //if the cards can be played
				SetListeners(i, temp);
			}
			else
			{
				temp.transform.GetChild(3).gameObject.SetActive(true); //otherwise black them out
			}
			i++;
		}
	}
	public void SetListeners(int where, GameObject temp)
	{ //sets all listeners on the cards
		temp.GetComponent<Button>().onClick.AddListener(() => {
			playedWild = handList[where].getNumb() >= 13;

			temp.GetComponent<Button>().onClick.RemoveAllListeners();
			Destroy(temp);
			TurnEnd(where);
		});
	}
	public void addCards(CardDisplay other)
	{ //recieves cards to add to the hand
		handList.Add(other);
	}
	public void RecieveDrawOnTurn()
	{ //if the player decides to draw
		handList[handList.Count - 1].loadCard(GameObject.Find("PlayerManager").GetComponent<PlayerManager>().PlayerArea.transform);
		drew = true;
		TurnEnd(-1);
	}
	public void TurnEnd(int where)
	{ //ends the player's turn
		PlayerManager cont = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

		cont.PlayerArea.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

		for (int i = cont.PlayerArea.transform.childCount - 1; i >= 0; i--)
		{
			cont.PlayerArea.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
			cont.PlayerArea.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
		}
		if (drew)
		{
			cont.GetComponent<PlayerManager>().enabled = true;
			//cont.recieveText(string.Format("{0} drew a card", name));
			//cont.deckGO.GetComponent<Button>().onClick.RemoveAllListeners();
		}
		else
		{
			int specNumb = handList[where].getNumb();
			if (playedWild)
			{
				//cont.updateDiscPile(handList[where]);
				handList.RemoveAt(where);
				//cont.startWild(name);
				//if (specNumb == 14)
					//cont.specialCardPlay(this, 14);
			}
			//else
			//{
			//	if (specNumb < 10)
			//	{
			//		cont.recieveText(string.Format("{0} played a {1} {2}", name, handList[where].getColor(), handList[where].getNumb()));
			//		cont.enabled = true;
			//	}
			//	else if (specNumb == 10)
			//	{
			//		cont.specialCardPlay(this, 10);
			//		cont.recieveText(string.Format("{0} played a {1} skip", name, handList[where].getColor()));
			//	}
			//	else if (specNumb == 11)
			//	{
			//		cont.specialCardPlay(this, 11);
			//		cont.recieveText(string.Format("{0} played a {1} reverse", name, handList[where].getColor()));
			//	}
			//	else if (specNumb == 12)
			//	{
			//		cont.specialCardPlay(this, 12);
			//		cont.recieveText(string.Format("{0} played a {1} draw 2", name, handList[where].getColor()));
			//	}
			//	cont.updateDiscPile(handList[where]);
			//	handList.RemoveAt(where);
			//}
		}

	}
	public bool Equals(IPlayerInterface other)
	{ //equals function based on name
		return other.getName().Equals(name);
	}
	public string getName()
	{ //returns the name
		return name;
	}
	public int getCardsLeft()
	{ //gets how many cards are left in the hand
		return handList.Count;
	}
}
