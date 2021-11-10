using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    
     GameObject CardBack;
     bool isFlipped = false;

    public void Flip()
    {
        CardBack = transform.Find("CardBack").gameObject;
        isFlipped = !isFlipped;
        // Cheap implementation so enemy cant see cards
        // not cheat proof tho
        if (isFlipped)
        {
            CardBack.SetActive(true); 
        }
        else
        {
            CardBack.SetActive(false);
        }

        // CardBack = (gameObject.GetComponent<Image>().sprite == CardFront) ? CardBack : CardFront;
    }
}
