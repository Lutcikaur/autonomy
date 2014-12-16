using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck1 : MonoBehaviour {

	// Use this for initialization

	public class Deck{
		private List<Card> _deck = new List<Card>();





	}

	/*
	deck RandomizeDeck(deck d){
		int i,j;
		int iMin;
		var r=new Random();
		for (i=0; i<deckSize; i++){
			d[i].randomVal=r.Next(1000);
		}
		deck TempDeck=new deck;

		// a[0] to a[n-1] is the array to sort 

		// advance the position through the entire array 
		//   (could do j < n-1 because single element is also min element) 
		for (j = 0; j < deckSize; j++) {
			// find the min element in the unsorted a[j .. n-1] 
			
			// assume the min is the first element 
			iMin = j;
			// test against elements after j to find the smallest 
			for ( i = j+1; i <= deckSize; i++) {
				// if this element is less, then it is the new minimum   
				if (d[i].randomVal < d[iMin].randomVal) {
					// found new minimum; remember its index 
					iMin = i;
				}
			}
			
			if(iMin != j) {
				//swaps d[j]& d[iMin];
				TempDeck[0].index=d[j].index;
				TempDeck[0].randomVal=d[j].randomVal;
				d[j].index=d[iMin].index;
				d[j].randomVal=d[iMin].randomVal;
				d[iMin].index=TempDeck[0];
				d[iMin].randomVal=TempDeck[0];
			}
			
		}



		return(d);
	}

	*/
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
