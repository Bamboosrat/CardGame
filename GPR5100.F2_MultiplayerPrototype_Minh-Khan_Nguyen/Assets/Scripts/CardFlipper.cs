using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    public Sprite CardFront;
    public Sprite CardBack;


    public void Flip()
    {
        Sprite currentSprite = gameObject.GetComponent<Image>().sprite;


        // Cheap implementation so enemy cant see cards
        // not cheat proof tho
        if(currentSprite == CardFront)
        {
            gameObject.GetComponent<Image>().sprite = CardBack;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = CardFront;
        }

        // other way to write this above
        // gameObject.GetComponent<Image>().sprite = (gameObject.GetComponent<Image>().sprite == CardFront) ? CardBack : CardFront;
    }
}
