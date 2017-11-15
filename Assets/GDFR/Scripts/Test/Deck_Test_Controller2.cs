using UnityEngine;

public class Deck_Test_Controller2 : MonoBehaviour {

	public Deck deckA = null;
	public Deck deckB = null;
	public Card testCard = null;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawAtoB()
	{
		Card card = deckA.DrawRandomCard() as Card;
		//StartCoroutine(card.AnimateDrawCard(deckB,1f));
		//deckB.AddCard(card);
		card.DrawCard(deckB);
		GameContoller.AI_PickBestCard(deckA,deckB);
		//RefreshAllCards();
	}

	public void DrawBtoA()
	{
		Card card = deckB.DrawRandomCard() as Card;
		//StartCoroutine(card.AnimateDrawCard(deckA,1f));
		//deckA.AddCard(card);
		card.DrawCard(deckA);
		//RefreshAllCards();
	}

}
