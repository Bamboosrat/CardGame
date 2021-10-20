using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInterface
{
	
		void turn();
		bool skipStatus
		{
			get;
			set;
		}
		void addCards(CardDisplay other);
		string getName();
		bool Equals(IPlayerInterface other);
		int getCardsLeft();
	
}
