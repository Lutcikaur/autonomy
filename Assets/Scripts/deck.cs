using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TechEnum {
	Tech0 = 0,
	Tech1=1,
	Tech2=2,
	Tech3=3,
	Tech4=4,
	Tech5=5
}

public class Card
{
		private TechEnum _tech;
		private int _rank;
		
		public TechEnum Suit { get { return _tech; } }
		public int Rank { get { return _rank; } }
		
		private GameObject _card;
		
		public Card(TechEnum level, int rank, Vector3 position, Quaternion rotation) {
			// to do: validate rank, position, and rotation
			string assetName = string.Format("Card_{0}_{1}", level, rank);  // Example:  "Card_1_10" would be the Jack of Hearts.
			GameObject asset = GameObject.Find(assetName);
			if (asset == null) {
				Debug.LogError("Asset '" + assetName + "' could not be found.");
			} else {
				//_card = Instantiate(asset, position, rotation);
				_tech = level;
				_rank = rank;
			}
		}
	}
	
public class Deck {
		private List<Card> _deck = new List<Card>();
		private List<Card> _discardPile = new List<Card>();
		
		public void Shuffle() {
			/* To Do */
		}
		
		public Card TakeCard() {
			if (_deck.Count == 0)
				return null; // the deck is depleted
			
			// take the first card off the deck and add it to the discard pile
			Card card = _deck[0];
			_deck.RemoveAt(0);
			_discardPile.Add(card);
			
			return card;
		}
		
		/* ...etc... */
}



