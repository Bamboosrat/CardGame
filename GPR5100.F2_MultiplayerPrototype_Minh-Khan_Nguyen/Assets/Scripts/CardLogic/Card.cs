using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card 
{
    public int number;
    public int Number => number;
    public CardColor color;
    public CardColor Color => color;


    public enum CardColor
    {
        green,
        red,
        blue,
        yellow,
        black
    }

    public enum SpecialCards
    {
        skip = 10,
        reverse = 11,
        draw2 = 12,
        wild = 13,
        wildDraw4 = 14
    }


    public Card() { }

    public Card(int number, int color) 
        : this(number, (CardColor) color)
    {
       
    }

    public Card(int Number, CardColor Color)
    {
        this.number = Number;
        this.color = Color;
    }

    [Server]
    public bool IsPlayable(Card other)
    {
        if (other.number == number || other.color == color)
            return true;
        else if (other.number == 13 || other.number == 14)
            return true;
        else
            return false;
    }

    public void ChangeColor(CardColor newColor)
    { //mutator that changes the color of a wild card to make the color noticable
        color = newColor;
    }

    public override string ToString()
    {
        return Number + "/" + Color;
    }

}
