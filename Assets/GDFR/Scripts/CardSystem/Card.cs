using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public interface IShufflable
{
	void Shuffle();
}

public interface ICard
{
	void Drawn();
	void Inspect();
	void Discard();
	void UpdateCardData();
	void Flip();
	Transform GetTransform();
	void LoadData(XmlNode root);
}

public class Card: MonoBehaviour, ICard
{

	public Deck parentDeck = null;

	public virtual void Drawn()
	{
		//Debug.Log("Card Drawn");
	}
	
	public virtual void Inspect()
	{
		//Debug.Log("Card Inspected");
	}
	
	public virtual void Discard()
	{
		//Debug.Log("Card Discared");
	}
	
	public virtual void UpdateCardData()
	{
		//Debug.Log("Card Data Update Activated");
	}
	
	public virtual void Flip()
	{
		//Debug.Log("Card Flipped");
	}
	
	public virtual void LoadData(XmlNode root)
	{
		//Debug.Log("Card LoadData");
	}	
	
	public virtual Transform GetTransform()
	{
		return transform;
	}


	public virtual void DrawCard(Deck toDeck,Transform parent = null)
	{
		//Debug.Log("Hit");
		Vector3 tempScale = transform.localScale;
		if(parent!=null)
			transform.parent = parent;
		else
			transform.parent = toDeck.gameObject.transform;
		transform.localScale = tempScale;
		transform.localPosition = Vector3.zero; 
	}



	//public virtual void DrawCard(Deck toDeck,out Transform newCardTranform)
	//{
	//	DrawCard(toDeck);
	//}
}

public class CardDeck<T> : List<T>,IShufflable
{
	public void Shuffle()
	{
	}
}
