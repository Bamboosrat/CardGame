using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
	* 1-9 are regular
	* 10 is skip
	* 11 is reverse
	* 12 is draw 2
	* 13 is wild
	* 14 is wild draw 4
	*/

public class CardDisplay
{
    int number;
    string color;
    public GameObject cardObj;

    public CardDisplay(int numb, string color, GameObject obj)
    { //defines the object
        number = numb;
        this.color = color;
		cardObj = obj;
		
    }

	public GameObject loadCard(int x, int y, Transform parent)
	{ //when ran, it tells where to load the card on the screen
		GameObject temp = loadCard(parent);
		temp.transform.localPosition = new Vector2(x, y + 540);
		return temp;
	}

	public GameObject loadCard(Transform parent)
	{ //does all the setup for loading. Used if card doesn't need a specific position		
		GameObject temp = GameManager.Instantiate(cardObj);
		
		temp.name = color + number;
		if (number < 10)
		{
			// Fuer jedes Child Component ausser "Cover" und "CardBack", veraendere die Werte im Textkomponent
			foreach (Transform childs in temp.transform)
			{
				if (childs.name.Equals("Cover") || childs.name.Equals("CardBack"))
					break;
				childs.GetComponent<Text>().text = number.ToString();
			}
			temp.transform.GetChild(1).GetComponent<Text>().color = returnColor(color);
		}
		//else if (number == 10 || number == 11 || number == 12)
		//{
		//	temp.transform.GetChild(1).GetComponent<RawImage>().color = returnColor(color);
		//}
		//else if (number == 13)
		//{
		//	temp.transform.GetChild(0).GetComponent<Text>().text = "";
		//	temp.transform.GetChild(2).GetComponent<Text>().text = "";
		//}

		// Veraendere die Farbe von der Karte abhaengig von "color"
		temp.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/" + color + "Card");
		temp.transform.SetParent(parent, false);
		temp.transform.localScale = new Vector3(1, 1, 1);

		
		return temp;
	}

	Color returnColor(string what)
	{ //returns a color based on the color string
		switch (what)
		{
			case "Green":
				return new Color32(0x55, 0xaa, 0x55, 255);
			case "Blue":
				return new Color32(0x55, 0x55, 0xfd, 255);
			case "Red":
				return new Color32(0xff, 0x55, 0x55, 255);
			case "Yellow":
				return new Color32(0xff, 0xaa, 0x00, 255);
		}
		return new Color(0, 0, 0);
	}


	public int getNumb()
	{ //accessor for getting the number
		return number;
	}
	public string getColor()
	{ //accessor for getting the color
		return color;
	}
	public bool Equals(CardDisplay other)
	{ //overides the original Equals so that color or number must be equal
		return other.getNumb() == number || other.getColor().Equals(color);
	}
	public void changeColor(string newColor)
	{ //mutator that changes the color of a wild card to make the color noticable
		color = newColor;
	}


}
