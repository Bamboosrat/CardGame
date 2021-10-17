using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// how does mirror and scriptable object work

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public Sprite cardFront;
    public Sprite cardBack;

    public int value;
    public string color;
}
