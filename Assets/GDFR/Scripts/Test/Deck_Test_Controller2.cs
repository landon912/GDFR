using UnityEngine;
using System.Collections;

public class Deck_Test_Controller2 : MonoBehaviour {

	public GDFR_Deck_Script deckA = null;
	public GDFR_Deck_Script deckB = null;
	public Card testCard = null;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawAtoB()
	{
		GDFR_Card_Script card = deckA.DrawRandomCard() as GDFR_Card_Script;
		//StartCoroutine(card.AnimateDrawCard(deckB,1f));
		//deckB.AddCard(card);
		card.DrawCard(deckB);
		GameContoller.AI_PickBestCard(deckA,deckB);
		//RefreshAllCards();
	}

	public void DrawBtoA()
	{
		GDFR_Card_Script card = deckB.DrawRandomCard() as GDFR_Card_Script;
		//StartCoroutine(card.AnimateDrawCard(deckA,1f));
		//deckA.AddCard(card);
		card.DrawCard(deckA);
		//RefreshAllCards();
	}

}
