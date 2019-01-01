using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    public int Id { get; set; }

    public int OwnerNetworkId = -1;

    private readonly List<Card> mCards = new List<Card>();
    private List<Card> mDrawableCards { get; set; } = new List<Card>();
    public Transform deckTransform;

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
            if (_VisuallyActive != value)
                SetVisuallyActive(value);
            _VisuallyActive = value;

        }
        get { return _VisuallyActive; }
    }
    public int _zDepth;
    public int zDepth
    {
        set
        {
            _zDepth = value;
            ChangeDeckDepth(_zDepth);
        }
        get { return _zDepth; }
    }

    public Transform deckPivot
    {
        get { return deckTransform; }
    }

    public bool playSparklesOnDraw = false;

    public Vector3 GetGridPosition(int index)
    {
        return deckGrid.GetGridPosition(index);
    }

    void Awake()
    {
        deckGrid = gameObject.GetComponentInChildren<UI_DeckGrid>();
        deckInActiveWidthLimit = deckGrid.widthLimit;
        deckTransform = deckGrid.gameObject.transform;
        if (xmlDataFile != null)
            LoadDeckData(xmlDataFile);
        Refresh();
    }

    void SetVisuallyActive(bool VisActive)
    {

        if (VisActive)
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

    public void Refresh()
    {
        deckGrid.Reposition();
    }

    public void CollapseDeck()
    {
        foreach (Transform t in transform)
        {
            t.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    //debug only
    public Card DrawAndMoveRandomCard(Deck toDeck)
    {
        if (mCards.Count > 0)
        {
            int randomTmp = Random.Range(0, mCards.Count);
            Debug.Log("Random card: " + randomTmp + " from " + mCards.Count);

            mCards[randomTmp].MoveToNewDeck(toDeck);
            randomTmp = Random.Range(0, 2);
            mCards[randomTmp].ChangeRace((Race)randomTmp);
            return mCards[randomTmp];
        }
        return null;
    }

    public Card DrawRandomCard()
    {
        if (mDrawableCards.Count > 0)
        {
            int randomTmp = Random.Range(0, mDrawableCards.Count);

            Card c = mDrawableCards[randomTmp];

            mDrawableCards.Remove(c);

            return c;
        }

        Debug.LogError("Failed to draw card");

        return null;
    }

    public Card DrawRandomCardOfSymbolGroup(SymbolGroup symbolGroup)
    {
        if (mDrawableCards.Count > 0)
        {
            List<Card> tempCards = new List<Card>();

            //sort cards
            foreach (Card c in mDrawableCards)
            {
                if (c.CurrentSymbolGroup == symbolGroup)
                {
                    tempCards.Add(c);
                }
            }

            if (tempCards.Count > 0)
            {
                int randomTmp = Random.Range(0, tempCards.Count);

                Card c = tempCards[randomTmp];

                if(mDrawableCards.Remove(c) == false)
                {
                    Debug.Log("remove failed");
                }

                return c;
            }
        }

        return null;
    }

    public Card GetExactCard(int id)
    {
        //do not check from drawable cards
        for (int i = 0; i < mCards.Count; i++)
        {
            Card c = mCards[i];

            if (c.Id == id)
            {
                //mCards.Remove(c);
                return c;
            }
        }

        Debug.LogError("Card " + id + " not found in deck " + Id);
        return null;
    }

    public Card[] GetCardList()
    {
        return mCards.ToArray();
    }

    public void DeckUiEnabled(bool isEnabled)
    {
        UIButton[] buttons = deckTransform.gameObject.GetComponentsInChildren<UIButton>();
        foreach (UIButton b in buttons)
        {
            b.enabled = isEnabled;
            b.GetComponent<Collider>().enabled = isEnabled;
        }
    }

    public void ReturnAllCards(Deck toDeck)
    {
        if (toDeck == null) return;

        Card[] cardList = GetCardList();
        foreach (Card c in cardList)
        {
            c.GetComponent<UIButton>().enabled = false;
            c.GetComponent<Collider>().enabled = false;
            c.DeckDepthOffset = 0;
            c.Depth = 0;
            c.MoveToNewDeck(toDeck);
        }
    }

    public Card AddCard(Card card)
    {
        Deck fromDeck = card.parentDeck;
        Transform newCardTrans = card.gameObject.transform;
        Vector3 tempScale = newCardTrans.localScale;

        if (fromDeck != null)
            fromDeck.RemoveCard(card);

        card.gameObject.transform.parent = deckTransform;
        card.parentDeck = this;

        mCards.Add(card);
        mDrawableCards.Add(card);

        EventReceiver.TriggerCardMovedEvent(card);

        Refresh();
        card.DeckDepthOffset = zDepth;

        if (playSparklesOnDraw)
            card.CardSparkleOverTime(0.4f);
        if (fromDeck != null)
            fromDeck.Refresh();
        return card;
    }

    public Card AddCardInstant(Card card)
    {
        if (card.parentDeck != null)
            card.parentDeck.RemoveCard(card);

        card.gameObject.transform.parent = deckTransform;
        card.parentDeck = this;

        mCards.Add(card);
        mDrawableCards.Add(card);

        card.transform.localPosition = Vector3.zero;
        card.DeckDepthOffset = zDepth;

        return card;
    }

    public Card RemoveCard(Card removeCard)
    {
        removeCard.gameObject.transform.parent = null;
        mCards.Remove(removeCard);
        mDrawableCards.Remove(removeCard);
        return removeCard;
    }

    void OnCardAdded(Card card)
    {
        Debug.Log("Card Added");
    }

    void OnCardRemoved(Card card)
    {
        Debug.Log("Card Removed");
    }

    private void ChangeDeckDepth(int newDepth)
    {
        for (int i = 0; i < mCards.Count; i++)
        {
            mCards[i].DeckDepthOffset = newDepth;
        }
    }

    public void LoadDeckData(Object xmlDataFile)
    {
        //Load
        TextAsset textXML = (TextAsset)Resources.Load(xmlDataFile.name, typeof(TextAsset));
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(textXML.text);

        //Read
        XmlNode root = xml.FirstChild;
        foreach (XmlNode node in root.ChildNodes)
        {
            if (node.Name == "card")
            {
                //new card
                GameObject newCard = Instantiate(cardPrefab);
                Card cardScript = newCard.GetComponent<Card>();
                AddCardInstant(cardScript);

                foreach (XmlNode cNode in node.ChildNodes)
                {
                    if (cNode.Name == "Id")
                    {
                        cardScript.Id = XmlConvert.ToInt32(cNode.InnerText);
                        cardScript.gameObject.name = "Card - Id: " + cardScript.Id;
                    }
                    if (cNode.Name == "GoblinSpriteName")
                    {
                        cardScript.goblinSpriteName = cNode.InnerText;
                    }
                    if (cNode.Name == "FairySpriteName")
                    {
                        cardScript.fairySpriteName = cNode.InnerText;
                    }
                    if (cNode.Name == "GoblinSymbol")
                    {
                        cardScript.goblinSymbol = (Symbol)Enum.Parse(typeof(Symbol), cNode.InnerText); ;
                    }
                    if (cNode.Name == "FairySymbol")
                    {
                        cardScript.fairySymbol = (Symbol)Enum.Parse(typeof(Symbol), cNode.InnerText); ;
                    }
                    if (cNode.Name == "GoblinRhyme")
                    {
                        cardScript.goblinRhyme = (Rhyme)Enum.Parse(typeof(Rhyme), cNode.InnerText); ;
                    }
                    if (cNode.Name == "FairyRhyme")
                    {
                        cardScript.fairyRhyme = (Rhyme)Enum.Parse(typeof(Rhyme), cNode.InnerText); ;
                    }
                    if (cNode.Name == "GoblinStarBorder")
                    {
                        cardScript.goblinStarBorder = XmlConvert.ToBoolean(cNode.InnerText);
                        if (!cardScript.starBorder && cardScript.goblinStarBorder)
                            cardScript.starBorder = true;
                    }
                    if (cNode.Name == "FairyStarBorder")
                    {
                        cardScript.fairyStarBorder = XmlConvert.ToBoolean(cNode.InnerText);
                        if (!cardScript.starBorder && cardScript.fairyStarBorder)
                            cardScript.starBorder = true;
                    }
                    if (cNode.Name == "GoblinText")
                    {
                        cardScript.goblinText = cNode.InnerText;
                    }
                    if (cNode.Name == "FairyText")
                    {
                        cardScript.fairyText = cNode.InnerText;
                    }
                    cardScript.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }
    }
}