using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*public enum TechEnum {
	Tech0=0,
	Tech1=1,
	Tech2=2,
	Tech3=3,
	Tech4=4,
	Tech5=5
}*/

//public class Card
//{
	/*	
	//private TechEnum _tech;
		private int _rank;
		
		//public TechEnum Suit { get { return _tech; } }
	public int Rank = 0; //{ get { return _rank; } }
		
<<<<<<< HEAD
		//private GameObject _card = Deck_0;
=======
		private GameObject _card;
>>>>>>> 07914a179a32015a49fcfb9909462d6dc003315d
		
		public Card(int rank, Vector3 position, Quaternion rotation) {
			// to do: validate rank, position, and rotation
		for(rank = 0; rank <=2; rank++){
			string assetName = string.Format("Deck_{0}", rank);  // Example:  "Card_1_10" would be the Jack of Hearts.
			GameObject asset = GameObject.Instantiate(_card) as GameObject;
			if (asset == null) {
				Debug.LogError("Asset '" + assetName + "' could not be found.");
			} else {
				//_card = GameObject.Instantiate(asset);
				//_tech = level;
				_rank = rank;
			}
		}
	}
<<<<<<< HEAD
}
	
=======
	*/
//}
	/*
>>>>>>> f5451dd5a501ac9ad733d43c76ba0d777407f342
public class Deck {
		private List<Card> _deck = new List<Card>();
		private List<Card> _discardPile = new List<Card>();
		
		public void Shuffle() {
			//todo
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
		
		//etc
}



*/


/*
public class deck : MonoBehaviour
{
	public List<GameObject> deck = new List<GameObject>();
	private List<GameObject> cards = new List<GameObject>();
	private List<GameObject> hand = new List<GameObject>();
	private int cardsDealt = 0;
	private bool showReset = false;
	
	void ResetDeck()
	{
		cardsDealt = 0;
		for (int i = 0; i < hand.Count; i++) {
			Destroy(hand[i]);
		}
		hand.Clear();
		cards.Clear();
		cards.AddRange(deck);
		showReset = false;
	}
	
	GameObject DealCard()
	{
		if(cards.Count == 0)
		{
			showReset = true;
			return null;
			//Alternatively to auto reset the deck:
			//ResetDeck();
		}
		
		int card = Random.Range(0, cards.Count - 1);
		GameObject go = GameObject.Instantiate(cards[card]) as GameObject;
		cards.RemoveAt(card);
		
		if(cards.Count == 0) {
			showReset = true;
		}
		
		return go;
	}
	
	void Start()
	{
		ResetDeck();
	}
	
	void GameOver()
	{
		cardsDealt = 0;
		for (int v = 0; v < hand.Count; v++) {
			Destroy(hand[v]);
		}
		hand.Clear();
		cards.Clear();
		cards.AddRange(deck);
	}
	
	void OnGUI()
	{
		if (!showReset) {
			// Deal button
			if (GUI.Button(new Rect(10, 10, 100, 20), "Deal"))
			{
				MoveDealtCard();
			}
		}
		else {
			// Reset button
			if (GUI.Button(new Rect(10, 10, 100, 20), "Reset"))
			{
				ResetDeck();
			}
		}
		// GameOver button
		if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 20), "GameOver"))
		{
			GameOver();
		}
	}
	
	void MoveDealtCard()
	{
		GameObject newCard = DealCard();
		// check card is null or not
		if (newCard == null) {
			Debug.Log("Out of Cards");
			showReset = true;
			return;
		}
		
		//newCard.transform.position = Vector3.zero;
		newCard.transform.position = new Vector3((float)cardsDealt / 4, (float)cardsDealt / -4, (float)cardsDealt / -4); // place card 1/4 up on all axis from last
		hand.Add(newCard); // add card to hand
		cardsDealt ++;
	}
}*/

