﻿using System.Collections;
using UnityEngine;

public class GDFR_Card_Script : Card
{
    private bool _cardSparkle;

    public bool _inspecting;

    //public TweenAlpha symbolGlowTweener = null;
    //public TweenScale symbolGlowScaleTweener = null;
    public int _zDepth;

    private int _zDepthOffset;
    public TweenScale cardBumpTweener = null;
    public TweenRotation cardFlipTweenerA = null;
    public TweenRotation cardFlipTweenerB = null;
    public TweenTransform cardLabelMoveOnFlipTweener = null;
    public TweenTransform cardMoveTweener = null;
    public TweenScale cardScaleTweener = null;

    public nEmitter cardSparkleEmitter = null;
    public GameObject defaultLabelPosition;
    public UILabel fadingFlipText;
    public Rhyme fairyRhyme;
    public string fairySpriteName = "";
    public bool fairyStarBorder = false;
    public Symbol fairySymbol;
    public string fairyText = "Fairy Test";
    public string FrogSpriteName = "";
    public Rhyme goblinRhyme;
    public string goblinSpriteName = "";
    public bool goblinStarBorder = false;

    public Symbol goblinSymbol;
    public string goblinText = "Goblin Test";
    private bool isFront = true;
    public GameObject labelPositionOnFlip;

    private Race mCurrentRace;

    private string mNameSound;
    public string MoonSpriteName = "";
    public string MushroomSpriteName = "";

    public Transform scaleTransform = null;
    public UISprite shadowSprite;

    public UISprite sprite;
    public bool starBorder = false;
    public playStars starsScript;
    public string SunSpriteName = "";
    public UISprite symbolGlowSprite;
    public UISprite symbolSprite;
    public UILabel text;
    private UITweener[] tweenerList;
    private int[] widgetDefaultDepth;
    private UIWidget[] widgetList;

    public bool StarsShowing
    {
        get
        {
            if (mCurrentRace == Race.Goblin && goblinStarBorder)
                return true;
            if (mCurrentRace == Race.Fairy && fairyStarBorder)
                return true;
            return false;
        }
    }

    public Race CurrentRace
    {
        set
        {
            if (value != mCurrentRace)
                ChangeRace(value);
        }
        get { return mCurrentRace; }
    }

    public Symbol CurrentSymbol { get; private set; }

    public Rhyme CurrentRhyme { get; private set; }

    public int zDepth
    {
        set
        {
            _zDepth = value;
            SetZDepth(_zDepth);
        }
        get { return _zDepth; }
    }

    public int zDepthOffset
    {
        set
        {
            _zDepthOffset = value;
            SetZDepth(_zDepth);
        }
        get { return _zDepthOffset; }
    }

    public bool cardSparkle
    {
        set
        {
            _cardSparkle = value;
            cardSparkleEmitter.enabled = _cardSparkle;
        }
        get { return _cardSparkle; }
    }


    // Use this for initialization
    private void Start()
    {
        ChangeRace(mCurrentRace);
        //mCurrentRace = Race.Fairy;
        var button = GetComponent<UIButton>();
        button.normalSprite = null;
    }

    private void Awake()
    {
        transform.localScale = scaleTransform.localScale = new Vector3(1, 1, 1);
        tweenerList = GetComponentsInChildren<UITweener>();
        cardSparkle = _cardSparkle;

        widgetList = GetComponentsInChildren<UIWidget>(true);

        //Capture default z
        widgetDefaultDepth = new int[widgetList.Length];
        for (var w = 0; w < widgetList.Length; w++)
            widgetDefaultDepth[w] = widgetList[w].depth;
    }

    public override void Flip()
    {
        base.Flip();
        EventReceiver.TriggerCardFlipEvent(this);
        StartCoroutine(AnimateFlip());
    }

    public IEnumerator AnimateFlip()
    {
        fadingFlipText.text = text.text;
        fadingFlipText.gameObject.SetActive(true);
        text.gameObject.SetActive(false);
        MoveLabel(labelPositionOnFlip.transform, 1f);

        if (isFront)
            cardFlipTweenerA.PlayForward();
        else
            cardFlipTweenerA.PlayReverse();

        yield return new WaitForSeconds(0.5f);

        text.gameObject.SetActive(true);

        isFront = !isFront;

        if (mCurrentRace == Race.Fairy)
        {
            CurrentRace = Race.Goblin;
            if (Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers)
            {
                text.text = ((int) CurrentRhyme).ToString();
                text.fontSize = 40;
            }
            else
            {
                text.text = goblinText;
                text.fontSize = 23;
            }
            SetSymbol(goblinSymbol);
        }
        else
        {
            CurrentRace = Race.Fairy;
            text.text = Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers
                ? ((int) CurrentRhyme).ToString()
                : fairyText;
            SetSymbol(fairySymbol);
        }

        scaleTransform.localScale = isFront ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);


        yield return new WaitForSeconds(0.5f);

        fadingFlipText.gameObject.SetActive(false);
    }

    private void SetSymbol(Symbol symbol)
    {
        switch (symbol)
        {
            case Symbol.Sun:
                symbolSprite.spriteName = SunSpriteName;
                break;
            case Symbol.Moon:
                symbolSprite.spriteName = MoonSpriteName;
                break;
            case Symbol.Mushroom:
                symbolSprite.spriteName = MushroomSpriteName;
                break;
            case Symbol.Frog:
                symbolSprite.spriteName = FrogSpriteName;
                break;
        }
        symbolGlowSprite.spriteName = symbolSprite.spriteName + "_Glow";
    }

    public IEnumerator AnimateDrawCard(Deck toDeck, float duration)
    {
        //Debug.Log("animate draw card");
        //start animation, when done execute draw card.
        //GDFR_Deck_Script GDeck = (GDFR_Deck_Script)toDeck;

        //GameObject toGo = GameObject.FindGameObjectWithTag("to");
        //Card cs= toGo.GetComponent<Card>();
        //cs.DrawCard(toDeck,GDeck.deckPivot);
        //toGo.transform.localScale = new Vector3(1f,1f,1f);
        //Vector3 newPos = GDeck.GetGridPosition(GDeck.count);
        //toGo.transform.localPosition = newPos;
        //toDeck.Refresh();
        //MoveCard(toGo.transform,duration);

        //GDeck.Refresh();
        //yield return new WaitForSeconds(duration);
        toDeck.AddCard(this);
        yield return new WaitForSeconds(duration);
        //scaleTransform.localScale = new Vector3(1f,1f,1f);
    }


    /*
	public IEnumerator Co_Animate_DrawCard(Deck toDeck,float duration)
	{
		Debug.Log("New animate draw card");

		//Step 1:  remove the card from the deck it's currently in.  add it to the target deck;
		parentDeck.RemoveCard(this);

		//Step 1:  Open an spot in the toDeck for the card to go into.

		//Step 3:  Animate the card (wait)

		//Step 4:  Add the card to the target deck.
	}
	*/

    public override void Inspect()
    {
        base.Inspect();
        _inspecting = !_inspecting;
    }

    public void ChangeRace(Race race)
    {
        switch (race)
        {
            case Race.Fairy:
                sprite.spriteName = fairySpriteName;
                mCurrentRace = Race.Fairy;
                CurrentRhyme = fairyRhyme;
                CurrentSymbol = fairySymbol;
                if (Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers)
                {
                    text.text = ((int) CurrentRhyme).ToString();
                    text.fontSize = 40;
                }
                else
                {
                    text.text = fairyText;
                    text.fontSize = 23;
                }

                //Debug.Log ("HIT  Race Set To Fairy");
                break;
            case Race.Goblin:
                sprite.spriteName = goblinSpriteName;
                mCurrentRace = Race.Goblin;
                CurrentRhyme = goblinRhyme;
                CurrentSymbol = goblinSymbol;
                if (Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers)
                {
                    text.text = ((int) CurrentRhyme).ToString();
                    text.fontSize = 40;
                }
                else
                {
                    text.text = goblinText;
                    text.fontSize = 23;
                }

                //Debug.Log ("Race Set To goblin");
                break;
        }
        SetSymbol(CurrentSymbol);
        starsScript.gameObject.SetActive(StarsShowing);
    }

    public void MoveCard(Transform to, float duration)
    {
        Move(to, duration, cardMoveTweener, transform);
    }

    public void MoveLabel(Transform to, float duration)
    {
        Move(to, duration, cardLabelMoveOnFlipTweener, defaultLabelPosition.transform);
    }

    public void Move(Transform to, float duration, TweenTransform tween, Transform from)
    {
        var toParent = to.parent;

        tween.from = from;
        tween.to = to;
        tween.duration = duration;

        tween.ResetToBeginning();
        tween.Play(true);
    }

    public override void DrawCard(Deck toDeck, Transform parent = null)
    {
        //base.DrawCard (toDeck);
        //GDFR_Deck_Script GtoDeck = (GDFR_Deck_Script)toDeck;
        //Vector3 tempScale = scaleTransform.localScale;
        var deckActive = toDeck.gameObject.activeSelf;
        toDeck.gameObject.SetActive(true);
        toDeck.AddCard(this);
        toDeck.gameObject.SetActive(deckActive);
    }

    public void DrawCardInstant(Deck toDeck, Transform parent = null)
    {
        var deckActive = toDeck.gameObject.activeSelf;
        toDeck.gameObject.SetActive(true);
        ((GDFR_Deck_Script) toDeck).AddCardInstant(this);
        toDeck.gameObject.SetActive(deckActive);
    }

    public void Bump()
    {
        cardBumpTweener.ResetToBeginning();
        cardBumpTweener.PlayForward();
    }

    private void SetZDepth(int z)
    {
        z += _zDepthOffset;

        for (var w = 0; w < widgetList.Length; w++)
            widgetList[w].depth = widgetDefaultDepth[w] + z;
    }

    public void SymbolMatchEffect()
    {
        PlayTweenGroup(1);
    }

    public void PlayTweenGroup(int index)
    {
        foreach (var t in tweenerList)
        {
            if (t.tweenGroup == index)
            {
                t.ResetToBeginning();
                t.PlayForward();
            }
        }
    }

    public void CardSparkleOverTime(float duration)
    {
        StartCoroutine(CO_CardSparkleOverTime(duration));
    }

    private IEnumerator CO_CardSparkleOverTime(float duration)
    {
        cardSparkle = true;
        yield return new WaitForSeconds(duration);
        cardSparkle = false;
    }

    public void PlayStarsEffect()
    {
        starsScript.enabled = true;
    }
}