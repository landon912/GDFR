using UnityEngine;
using System.Collections;
using System.Xml;


public class GDFR_Deck_Script : Deck {
	
	public Object xmlDataFile;
	public GameObject cardPrefab;
	public TweenScale deckScaleTweener = null;
	UI_DeckGrid deckGrid;
	public bool _VisuallyActive = false;
	public float deckActiveWidthLimit = 600f;
	float deckInActiveWidthLimit = 600f;
	public bool VisuallyActive
	{
		set
		{
			if(_VisuallyActive!=value)
				SetVisuallyActive(value);
			_VisuallyActive = value;

		}
		get{return _VisuallyActive;}
	}
	public int _zDepth = 0;
	public int zDepth
	{
		set
		{
			_zDepth = value;
			SetZDepthOffset(_zDepth);
		}
		get{return _zDepth;}
	}

	public int count
	{
		get{return deckTransform.childCount;}
	}

	public Transform deckPivot
	{
		get{return deckTransform;}
	}

	public bool playSparklesOnDraw = true;

	public Vector3 GetGridPosition(int index)
	{
		return deckGrid.GetGridPosition(index);
	}

	void Awake()
    {
		deckGrid = gameObject.GetComponentInChildren<UI_DeckGrid>() as UI_DeckGrid;
		deckInActiveWidthLimit = deckGrid.widthLimit;
		deckTransform = deckGrid.gameObject.transform;
		if(xmlDataFile!=null)
			loadDeckData(xmlDataFile);
		Refresh();

	}

	void SetVisuallyActive(bool VisActive)
	{

		if(VisActive)
		{
			deckScaleTweener.PlayForward();
			deckGrid.widthLimit = deckActiveWidthLimit;
			deckGrid.Reposition();
		}
		else
		{
			deckScaleTweener.PlayReverse();
			deckGrid.widthLimit = deckInActiveWidthLimit;
			deckGrid.Reposition();
		}
	}
	
	public override void Refresh ()
	{
		base.Refresh ();
		deckGrid.Reposition();
	}

	public void CollapseDeck()
	{
		foreach(Transform t in transform)
		{
			t.localPosition = new Vector3(0f,0f,0f);
		}
	}
	
	public override Object drawRandomCard (Deck toDeck)
	{
        base.drawRandomCard(toDeck);
		int count = transform.childCount;
		if(count>0)
		{
            int randomTmp = Random.Range(0, count);
            Debug.Log("Random card: " + randomTmp.ToString() + " from " + count.ToString());
            Transform t = deckTransform.GetChild(randomTmp);
			GDFR_Card_Script card = t.gameObject.GetComponent<GDFR_Card_Script>();
			card.DrawCard(toDeck);
            randomTmp = Random.Range(0, 2);
            card.ChangeRace((Race)randomTmp);
			return card;
		}
		else
			return null;			
	}

	public override Object drawRandomCard ()
	{
        base.drawRandomCard();
		int count = deckTransform.childCount;
		if(count>0)
		{
            int randomTmp = Random.Range(0, count);
            Debug.Log("Random card: " + randomTmp.ToString() + " from " + count.ToString());
            Transform t = deckTransform.GetChild(randomTmp);
			GDFR_Card_Script card = t.gameObject.GetComponent<GDFR_Card_Script>();
			return card;
		}
		else
			return null;			
	}

	public Card[] GetCardList()
	{
		/*
		Card[] cl = new Card[transform.childCount];
		for(int t=0;t<=transform.childCount-1;t++)
		{
			cl[t] = transform.GetChild(t).gameObject.GetComponent<GDFR_Card_Script>();
		}
		*/
		GDFR_Card_Script[] cards = GetComponentsInChildren<GDFR_Card_Script>();
		return cards;
	}
	
	public void DeckUiEnabled(bool enabled)
	{
		UIButton[] buttons = deckTransform.gameObject.GetComponentsInChildren<UIButton>();
		foreach(UIButton b in buttons)
		{
			b.enabled = enabled;
			b.GetComponent<Collider>().enabled = enabled;
		}
	}

	public override void ReturnAllCards (Deck toDeck)
	{
		if(toDeck==null)return;
		base.ReturnAllCards (toDeck);
		Card[] cardList = GetCardList();
		foreach(Card c in cardList)
		{
			c.DrawCard(toDeck);
		}
	}

	public override Card AddCard(Card card)
	{
		Deck fromDeck = card.parentDeck;
		Transform newCardTrans = card.gameObject.transform;
		Vector3 tempScale = newCardTrans.localScale;

		base.AddCard(card);

		EventReceiver.TriggerCardMovedEvent((GDFR_Card_Script)card);

		//newCardTrans.localScale = tempScale;
		//newCardTrans.localPosition = GetGridPosition(deckTransform.childCount-1);
		Refresh();
		SetZDepthOffset(_zDepth);
		GDFR_Card_Script gCard = (GDFR_Card_Script)card;
		gCard.zDepth = _zDepth + 100;
		if(playSparklesOnDraw)
			gCard.CardSparkleOverTime(0.4f);
		if(fromDeck!=null)
			fromDeck.Refresh();
		return card;
	}

	public Card AddCardInstant(Card card)
	{
		base.AddCard(card);
		card.transform.localPosition = Vector3.zero;
		SetZDepthOffset(_zDepth);
		GDFR_Card_Script gCard = (GDFR_Card_Script)card;
		gCard.zDepth = _zDepth + 100;
		return card;
	}


	void OnCardAdded(Card card)
	{
		Debug.Log ("Card Added");
	}

	void OnCardRemoved(Card card)
	{
		Debug.Log("Card Removed");
	}

	public void SetZDepthOffset(int z)
	{
		GDFR_Card_Script[] cards = GetCardList() as GDFR_Card_Script[];
		foreach(GDFR_Card_Script card in cards)
			card.zDepthOffset = z;
	}

	public override void loadDeckData (Object xmlDataFile)
	{
		base.loadDeckData (xmlDataFile);

        //Load
        TextAsset textXML = (TextAsset)Resources.Load(xmlDataFile.name, typeof(TextAsset));
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(textXML.text);

        //Read
        XmlNode root = xml.FirstChild;
        foreach(XmlNode node in root.ChildNodes)
        {
			if(node.Name=="card")
			{
				//new card
				GameObject newCard = GameObject.Instantiate(cardPrefab) as GameObject;
				//newCard.transform.parent = deckTransform;
				//newCard.transform.position = Vector3.zero;
				GDFR_Card_Script CardScript = newCard.GetComponent<GDFR_Card_Script>();
				AddCardInstant(CardScript);
				foreach(XmlNode cNode in node.ChildNodes)
				{
	                if(cNode.Name=="GoblinSpriteName")
					{
						CardScript.goblinSpriteName = cNode.InnerText;
					}
	                if(cNode.Name=="FairySpriteName")
					{
						CardScript.fairySpriteName = cNode.InnerText;
					}	
	                if(cNode.Name=="GoblinSymbol")
					{
						CardScript.goblinSymbol =  (Symbol)System.Enum.Parse(typeof(Symbol), cNode.InnerText);;
					}						
	                if(cNode.Name=="FairySymbol")
					{
						CardScript.fairySymbol =  (Symbol)System.Enum.Parse(typeof(Symbol), cNode.InnerText);;
					}	
	                if(cNode.Name=="GoblinRhyme")
					{
						CardScript.goblinRhyme =  (Rhyme)System.Enum.Parse(typeof(Rhyme), cNode.InnerText);;
					}	
	                if(cNode.Name=="FairyRhyme")
					{
						CardScript.fairyRhyme =  (Rhyme)System.Enum.Parse(typeof(Rhyme), cNode.InnerText);;
					}	
					if(cNode.Name=="GoblinStarBorder")
					{
						CardScript.goblinStarBorder =  XmlConvert.ToBoolean(cNode.InnerText);
						if(!CardScript.starBorder && CardScript.goblinStarBorder)
							CardScript.starBorder = true;
					}			
					if(cNode.Name=="FairyStarBorder")
					{
						CardScript.fairyStarBorder =  XmlConvert.ToBoolean(cNode.InnerText);
						if(!CardScript.starBorder && CardScript.fairyStarBorder)
							CardScript.starBorder = true;
					}	
					if(cNode.Name=="GoblinText")
					{
						CardScript.goblinText =  cNode.InnerText;
					}	
					if(cNode.Name=="FairyText")
					{
						CardScript.fairyText =  cNode.InnerText;
					}	
					CardScript.gameObject.transform.localScale = new Vector3(1f,1f,1f);
				}
			}
        }	
	}
}
