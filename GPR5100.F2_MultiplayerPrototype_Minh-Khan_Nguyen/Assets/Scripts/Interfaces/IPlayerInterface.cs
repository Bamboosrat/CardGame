using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInterface
{
		// Options what to do when it's users turn
		void Turn();

		// Was the Skip card played and am I skipped?
		bool SkipStatus
		{
			get;
			set;
		}

		// add cards to my hand
		void AddCard(Card other);

		void RemoveCard(Card other);
		
		// Get name of player
		string GetName();

		// is the other player == ?
		bool Equals(PlayerManager other);

		// How many cards do I have left?
		int GetCardsLeft();
	
}
