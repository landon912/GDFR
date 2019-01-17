using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GDFR
{
    public class Deck : MonoBehaviour
    {
        public int Id { get; set; }

        public int OwnerNetworkId = -1;

        private readonly List<Card> mCards = new List<Card>();
        private List<Card> mDrawableCards { get; set; } = new List<Card>();

        private RectTransform mLocalRectTransform;

        public RectTransform LocalRectTransform
        {
            get
            {
                if (mLocalRectTransform == null) { mLocalRectTransform = GetComponent<RectTransform>(); }

                return mLocalRectTransform;
            }
        }

        public Object xmlDataFile;

        public GameObject cardPrefab;

        //public TweenScale deckScaleTweener = null;
        public DeckPositioner deckPositioner;
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

        public RectTransform mDeckPivot;

        public RectTransform DeckPivot
        {
            get { return mDeckPivot; }
            private set { mDeckPivot = value; }
        }

        public bool playSparklesOnDraw = false;

        //public Vector3 GetGridPosition(int index)
        //{
        //    return deckPositioner.GetGridPosition(index);
        //}

        void Awake()
        {
            DeckPivot = transform.GetChild(0).GetComponent<RectTransform>();

            if (xmlDataFile != null)
                LoadDeckData(xmlDataFile);
            //RefreshDeckPosition();
        }

        void SetVisuallyActive(bool VisActive)
        {

            if (VisActive)
            {
                DeckPivot.LeanScale(Vector3.one, 2).setEase(DeckPivot.GetComponent<TweenCurve>().tweenCurves[0]);
                deckPositioner.SetPosition(2.0f);
            }
            else
            {
                DeckPivot.LeanScale(new Vector3(0.6f, 0.6f, 0.6f), 2)
                    .setEase(DeckPivot.GetComponent<TweenCurve>().tweenCurves[0]);
                deckPositioner.SetPosition(2.0f);
            }
        }

        //public void RefreshDeckPosition()
        //{
        //    deckPositioner.SetPosition();
        //}

        public void CollapseDeck()
        {
            foreach (RectTransform t in transform)
            {
                t.anchoredPosition = Vector3.zero;
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
                mCards[randomTmp].ChangeRace((Race) randomTmp);
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

                    if (mDrawableCards.Remove(c) == false)
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
            Button[] buttons = DeckPivot.gameObject.GetComponentsInChildren<Button>();
            foreach (Button b in buttons)
            {
                b.interactable = isEnabled;
            }
        }

        public void ReturnAllCards(Deck toDeck)
        {
            if (toDeck == null)
            {
                Debug.LogError("Cannot call this method on a null deck");
                return;
            }
            if(toDeck == this)
            {
                Debug.LogError("Cannot return cards to the same deck.");
                return;
            }

            Card[] cardList = GetCardList();
            foreach (Card c in cardList)
            {
                c.GetComponent<Button>().interactable = false;
                c.DeckDepthOffset = 0;
                c.Depth = 0;
                c.MoveToNewDeckInstant(toDeck);
            }
        }

        public Card AddCard(Card card)
        {
            card.ParentDeck?.RemoveCard(card);

            card.LocalRectTransform.SetParent(DeckPivot);

            card.ParentDeck = this;

            mCards.Add(card);
            mDrawableCards.Add(card);

            EventReceiver.TriggerCardMovedEvent(card);

            card.DeckDepthOffset = zDepth;

            if (playSparklesOnDraw)
                card.CardSparkleOverTime(0.4f);

            return card;
        }

        public Card AddCardInstant(Card card)
        {
            card.ParentDeck?.RemoveCard(card);

            card.LocalRectTransform.SetParent(DeckPivot);
            card.ParentDeck = this;

            mCards.Add(card);
            mDrawableCards.Add(card);

            card.LocalRectTransform.anchoredPosition = Vector3.zero;
            card.DeckDepthOffset = zDepth;

            return card;
        }

        private void RemoveCard(Card removeCard)
        {
            mCards.Remove(removeCard);
            mDrawableCards.Remove(removeCard);
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
            TextAsset textXML = (TextAsset) Resources.Load(xmlDataFile.name, typeof(TextAsset));
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
                            cardScript.GoblinSpriteName = cNode.InnerText;
                        }

                        if (cNode.Name == "FairySpriteName")
                        {
                            cardScript.FairySpriteName = cNode.InnerText;
                        }

                        if (cNode.Name == "GoblinSymbol")
                        {
                            cardScript.GoblinSymbol = (Symbol) Enum.Parse(typeof(Symbol), cNode.InnerText);
                            ;
                        }

                        if (cNode.Name == "FairySymbol")
                        {
                            cardScript.FairySymbol = (Symbol) Enum.Parse(typeof(Symbol), cNode.InnerText);
                            ;
                        }

                        if (cNode.Name == "GoblinRhyme")
                        {
                            cardScript.GoblinRhyme = (Rhyme) Enum.Parse(typeof(Rhyme), cNode.InnerText);
                            ;
                        }

                        if (cNode.Name == "FairyRhyme")
                        {
                            cardScript.FairyRhyme = (Rhyme) Enum.Parse(typeof(Rhyme), cNode.InnerText);
                            ;
                        }

                        if (cNode.Name == "GoblinStarBorder")
                        {
                            cardScript.GoblinStarBorder = XmlConvert.ToBoolean(cNode.InnerText);
                            if (!cardScript.StarBorder && cardScript.GoblinStarBorder)
                                cardScript.StarBorder = true;
                        }

                        if (cNode.Name == "FairyStarBorder")
                        {
                            cardScript.FairyStarBorder = XmlConvert.ToBoolean(cNode.InnerText);
                            if (!cardScript.StarBorder && cardScript.FairyStarBorder)
                                cardScript.StarBorder = true;
                        }

                        if (cNode.Name == "GoblinText")
                        {
                            cardScript.GoblinText = cNode.InnerText;
                        }

                        if (cNode.Name == "FairyText")
                        {
                            cardScript.FairyText = cNode.InnerText;
                        }

                        cardScript.LocalRectTransform.localScale = new Vector3(1f, 1f, 1f);
                    }
                }
            }
        }
    }
}