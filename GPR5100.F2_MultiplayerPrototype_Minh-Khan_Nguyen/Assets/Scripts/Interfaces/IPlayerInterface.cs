using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInterface
{
		// Options what to do when it's users turn
		void turn();

		// Was the Skip card played and am I skipped?
		bool skipStatus
		{
			get;
			set;
		}

		// add cards to my hand
		void addCards(CardDisplay other);
		
		// Get name of player
		string getName();

		// is the other player == ?
		bool Equals(IPlayerInterface other);

		// How many cards do I have left?
		int getCardsLeft();
	
}
