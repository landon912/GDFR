using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GDFR_Deck_Script : Deck
{
    private List<GDFR_Card_Script> cards = new List<GDFR_Card_Script>();

    public Object xmlDataFile;
	public GameObject cardPrefab;
	public TweenScale deckScaleTweener = null;
	UI_DeckGrid deckGrid;
	public bool _VisuallyActive;
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
	public int _zDepth;
	public int zDepth
	{
		set
		{
			_zDepth = value;
			SetZDepthOffset(_zDepth);
		}
		get{return _zDepth;}
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
		deckGrid = gameObject.GetComponentInChildren<UI_DeckGrid>();
		deckInActiveWidthLimit = deckGrid.widthLimit;
		deckTransform = deckGrid.gameObject.transform;
		if(xmlDataFile!=null)
			LoadDeckData(xmlDataFile);
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
	
	public override Card DrawRandomCard (Deck toDeck)
	{
        base.DrawRandomCard(toDeck);
		int count = transform.childCount;
		if(count>0)
		{
            int randomTmp = Random.Range(0, count);
            Debug.Log("Random card: " + randomTmp + " from " + count);
            Transform t = deckTransform.GetChild(randomTmp);
			GDFR_Card_Script card = t.gameObject.GetComponent<GDFR_Card_Script>();
			card.DrawCard(toDeck);
            randomTmp = Random.Range(0, 2);
            card.ChangeRace((Race)randomTmp);
			return card;
		}
	    return null;
	}

	public override Card DrawRandomCard ()
	{
        base.DrawRandomCard();
		if(cards.Count >0)
		{
            int randomTmp = Random.Range(0, cards.Count);
            Debug.Log("Random card: " + randomTmp + " from " + cards.Count);
            
			return cards[randomTmp];
		}

        return null;
	}

    public GDFR_Card_Script DrawRandomCardOfSymbolGroup(SymbolGroup symbolGroup)
    {
        if (cards.Count > 0)
        {
            List<GDFR_Card_Script> tempCards = new List<GDFR_Card_Script>();

            //sort cards
            foreach (GDFR_Card_Script c in cards)
            {
                if (c.CurrentSymbolGroup == symbolGroup)
                {
                    tempCards.Add(c);
                }
            }

            if (tempCards.Count > 0)
            {
                int randomTmp = Random.Range(0, tempCards.Count);
                Debug.Log("Random card of symbol group " + symbolGroup + ": " + randomTmp + " from " + tempCards.Count);

                return tempCards[randomTmp];
            }
        }

        return null;
    }

    public GDFR_Card_Script[] GetCardList()
    {
        return cards.ToArray();
    }
	
	public void DeckUiEnabled(bool isEnabled)
	{
		UIButton[] buttons = deckTransform.gameObject.GetComponentsInChildren<UIButton>();
		foreach(UIButton b in buttons)
		{
			b.enabled = isEnabled;
			b.GetComponent<Collider>().enabled = isEnabled;
		}
	}

	public override void ReturnAllCards (Deck toDeck)
	{
		if(toDeck==null)return;
		base.ReturnAllCards (toDeck);
		GDFR_Card_Script[] cardList = GetCardList();
		foreach(GDFR_Card_Script c in cardList)
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
	    GDFR_Card_Script gCard = (GDFR_Card_Script)card;
        cards.Add(gCard);

        EventReceiver.TriggerCardMovedEvent((GDFR_Card_Script)card);

		Refresh();
		SetZDepthOffset(_zDepth);
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
	    GDFR_Card_Script gCard = (GDFR_Card_Script)card;
        cards.Add(gCard);

        card.transform.localPosition = Vector3.zero;
		SetZDepthOffset(_zDepth);

		gCard.zDepth = _zDepth + 100;

		return card;
	}

    public override Card RemoveCard(Card removeCard)
    {
        base.RemoveCard(removeCard);
        cards.Remove((GDFR_Card_Script)removeCard);
        return removeCard;
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

	public override void LoadDeckData (Object xmlDataFile)
	{
		base.LoadDeckData (xmlDataFile);

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
				GameObject newCard = Instantiate(cardPrefab);
				GDFR_Card_Script cardScript = newCard.GetComponent<GDFR_Card_Script>();
				AddCardInstant(cardScript);
				foreach(XmlNode cNode in node.ChildNodes)
				{
	                if(cNode.Name=="GoblinSpriteName")
					{
						cardScript.goblinSpriteName = cNode.InnerText;
					}
	                if(cNode.Name=="FairySpriteName")
					{
						cardScript.fairySpriteName = cNode.InnerText;
					}	
	                if(cNode.Name=="GoblinSymbol")
					{
						cardScript.goblinSymbol =  (Symbol)Enum.Parse(typeof(Symbol), cNode.InnerText);;
					}						
	                if(cNode.Name=="FairySymbol")
					{
						cardScript.fairySymbol =  (Symbol)Enum.Parse(typeof(Symbol), cNode.InnerText);;
					}	
	                if(cNode.Name=="GoblinRhyme")
					{
						cardScript.goblinRhyme =  (Rhyme)Enum.Parse(typeof(Rhyme), cNode.InnerText);;
					}	
	                if(cNode.Name=="FairyRhyme")
					{
						cardScript.fairyRhyme =  (Rhyme)Enum.Parse(typeof(Rhyme), cNode.InnerText);;
					}	
					if(cNode.Name=="GoblinStarBorder")
					{
						cardScript.goblinStarBorder =  XmlConvert.ToBoolean(cNode.InnerText);
						if(!cardScript.starBorder && cardScript.goblinStarBorder)
							cardScript.starBorder = true;
					}			
					if(cNode.Name=="FairyStarBorder")
					{
						cardScript.fairyStarBorder =  XmlConvert.ToBoolean(cNode.InnerText);
						if(!cardScript.starBorder && cardScript.fairyStarBorder)
							cardScript.starBorder = true;
					}	
					if(cNode.Name=="GoblinText")
					{
						cardScript.goblinText =  cNode.InnerText;
					}	
					if(cNode.Name=="FairyText")
					{
						cardScript.fairyText =  cNode.InnerText;
					}	
					cardScript.gameObject.transform.localScale = new Vector3(1f,1f,1f);
				}
			}
        }	
	}
}
