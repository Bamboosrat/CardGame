using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    
     GameObject CardBack;
   // bool isFlipped = false;

    public void Flip()
    {
       // CardBack = transform.GetChild(5).GetComponent<GameObject>();
       //
       //
       // // Cheap implementation so enemy cant see cards
       // // not cheat proof tho
       // if(isFlipped)
       // {
       //     CardBack.SetActive(true);
       //
       //     isFlipped = !isFlipped;
       // }
       // else
       // {
       //     CardBack.SetActive(false);
       // }
       // Debug.Log(isFlipped);

        // other way to write this above
        // gameObject.GetComponent<Image>().sprite = (gameObject.GetComponent<Image>().sprite == CardFront) ? CardBack : CardFront;
    }
}
