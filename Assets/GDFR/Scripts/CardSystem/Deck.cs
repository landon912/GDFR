using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	
	public Transform deckTransform = null;

	void Awake()
	{
		if(deckTransform==null)
			deckTransform = transform;
	}

	public virtual Object drawRandomCard(Deck toDeck)
	{
		return null;
	}

	public virtual Object drawRandomCard()
	{
		return null;
	}

	public virtual void loadDeckData(Object xmlDataFile)
	{
	}

	public virtual void Refresh()
	{
	}

	public virtual void ReturnAllCards(Deck toDeck)
	{
	}

	public virtual Card AddCard(Card card)
	{
		if(card.parentDeck!=null)
			card.parentDeck.RemoveCard(card);


		//Debug.Log("Card Add Called");

		card.gameObject.transform.parent = deckTransform;
		card.parentDeck = this;
		return card;
	}

	public virtual Card RemoveCard(Card card)
	{
		//Debug.Log("Card Remove Called");
		card.gameObject.transform.parent = null;
		return card;
	}

}
