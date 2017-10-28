using UnityEngine;
using System.Collections;

public enum Race {Goblin,Fairy};
public enum Symbol {Sun, Moon, Mushroom, Frog};
public enum Rhyme {Our = 1, Oop = 2, Ock = 3, Elly = 4, Ew = 5};

public class GDFR_Card_Script : Card {

	public UISprite sprite;
	public UISprite shadowSprite;
	public UILabel text;
    public UILabel fadingFlipText;
    public UISprite symbolSprite;
	public UISprite symbolGlowSprite;
	public playStars starsScript;
	public string goblinSpriteName = "";
	public string fairySpriteName = "";
	public string SunSpriteName = "";
	public string MoonSpriteName = "";
	public string MushroomSpriteName = "";
	public string FrogSpriteName = "";
	public Symbol goblinSymbol;
	public Symbol fairySymbol;
	public Rhyme goblinRhyme;
	public Rhyme fairyRhyme;
	public string goblinText = "Goblin Test";
	public string fairyText = "Fairy Test";
	public Race _currentRace;
	public bool _inspecting = false;
	public bool goblinStarBorder = false;
	public bool fairyStarBorder = false;
	public bool starBorder = false;
	bool isFront = true;
    public GameObject defaultLabelPosition;
    public GameObject labelPositionOnFlip;

    public bool starsShowing
	{
		get
		{
			if(_currentRace==Race.Goblin && goblinStarBorder)
				return true;
			if(_currentRace==Race.Fairy && fairyStarBorder)
				return true;
			return false;
		}
	}
	public Race currentRace
	{
		set
		{
			if(value != _currentRace)
			{
				ChangeRace(value);
			}
		}
		get{return _currentRace;}
	}
	public Symbol _currentSymbol;
	public Symbol currentSymbol
	{
		get{return _currentSymbol;}
	}
	public Rhyme _currentRhyme;
	public Rhyme currentRhyme
	{
		get{return _currentRhyme;}
	}	
	public Transform scaleTransform = null;
	public TweenTransform cardMoveTweener = null;
	public TweenScale cardScaleTweener = null;
	public TweenRotation cardFlipTweenerA = null;
	public TweenRotation cardFlipTweenerB = null;
    public TweenTransform cardLabelMoveOnFlipTweener = null;
    public TweenScale cardBumpTweener = null;
	//public TweenAlpha symbolGlowTweener = null;
	//public TweenScale symbolGlowScaleTweener = null;
	public int _zDepth = 0;
	UITweener[] tweenerList = null;
	public int zDepth
	{
		set
		{
			_zDepth = value;
			setZdepth(_zDepth);
		}
		get{return _zDepth;}
	}
	int _zDepthOffset = 0;
	public int zDepthOffset
	{
		set
		{
			_zDepthOffset = value;
			setZdepth(_zDepth);
		}
		get{return _zDepthOffset;}
	}
	bool _cardSparkle = false;
	public bool cardSparkle
	{
		set
		{
			_cardSparkle = value;
			cardSparkleEmitter.enabled=_cardSparkle;
		}
		get{return _cardSparkle;}
	}

	public nEmitter cardSparkleEmitter = null;
	UIWidget[] widgetList;
	int[] widgetDefaultDepth;


	// Use this for initialization
	void Start () {
		ChangeRace(_currentRace);
		//currentRace = Race.Fairy;
		UIButton button = GetComponent<UIButton>();
		button.normalSprite = null;
	}

	void Awake() {
		transform.localScale = scaleTransform.localScale = new Vector3(1,1,1);
		tweenerList = GetComponentsInChildren<UITweener>();
		cardSparkle = _cardSparkle;
		widgetList = GetComponentsInChildren<UIWidget>();
		//Capture deafault z
		widgetDefaultDepth = new int[widgetList.Length];
		for(int w=0;w<widgetList.Length;w++)
		{
			widgetDefaultDepth[w]=widgetList[w].depth;
		}

	}
	
	public override void Flip ()
	{
		base.Flip ();
		StartCoroutine(AnimateFlip());
	}

    public IEnumerator AnimateFlip ()
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

		if(_currentRace==Race.Fairy)
		{
			currentRace = Race.Goblin;
            if (Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers)
            {
                text.text = ((int)_currentRhyme).ToString();
                text.fontSize = 40;
            }
            else
            {
                text.text = goblinText;
                text.fontSize = 23;
            }
			setSymbol(goblinSymbol);
		}
		else
		{
			currentRace = Race.Fairy;
            text.text = Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers ? ((int)_currentRhyme).ToString() : fairyText;
			setSymbol(fairySymbol);
		}

		scaleTransform.localScale = isFront ? new Vector3(1,1,1) : new Vector3(-1,1,1);


		yield return new WaitForSeconds(0.5f);

	    fadingFlipText.gameObject.SetActive(false);
    }

	void setSymbol(Symbol symbol)
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

	public IEnumerator AnimateDrawCard(Deck toDeck,float duration)
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

	public override void Inspect ()
	{
		base.Inspect ();
		_inspecting = !_inspecting;
	}
	
	public void ChangeRace(Race race)
	{
		switch (race)
		{
			case Race.Fairy:
				sprite.spriteName = fairySpriteName;
				_currentRace = Race.Fairy;
				_currentRhyme = fairyRhyme;
				_currentSymbol = fairySymbol;
                if (Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers)
                {
                    text.text = ((int)_currentRhyme).ToString();
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
				_currentRace = Race.Goblin;
				_currentRhyme = goblinRhyme;
				_currentSymbol = goblinSymbol;
                if (Toolbox.Instance.gameSettings.cardVariant == GameSettings.CardVariant.Numbers)
                {
                    text.text = ((int)_currentRhyme).ToString();
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
		setSymbol(_currentSymbol);
		starsScript.gameObject.SetActive(starsShowing);
	}
	
	void OnClick()
	{
		//Debug.Log("Clicked");
		//Flip();
	}
	
	void OnHover()
	{
		//Inspect();
	}

	public void SwapRace()
	{
		if(_currentRace==Race.Goblin)
			ChangeRace(Race.Fairy);
		else
			ChangeRace(Race.Goblin);
	}

	public void MoveCard(Transform to,float duration)
	{
        Move(to, duration, cardMoveTweener, this.transform);
	}

    public void MoveLabel(Transform to, float duration)
    {
        Move(to, duration, cardLabelMoveOnFlipTweener, defaultLabelPosition.transform);
    }

    public void Move(Transform to, float duration, TweenTransform tween, Transform from)
    {
        Transform toParent = to.parent;

        tween.from = from;
        tween.to = to;
        tween.duration = duration;

        tween.ResetToBeginning();
        tween.Play(true);
    }

    public override void DrawCard (Deck toDeck, Transform parent = null)
	{
		//base.DrawCard (toDeck);
		//GDFR_Deck_Script GtoDeck = (GDFR_Deck_Script)toDeck;
		//Vector3 tempScale = scaleTransform.localScale;
		bool deckActive = toDeck.gameObject.activeSelf;
		toDeck.gameObject.SetActive(true);
		toDeck.AddCard(this);
		toDeck.gameObject.SetActive(deckActive);
	}

	public void DrawCardInstant (Deck toDeck, Transform parent = null)
	{
		bool deckActive = toDeck.gameObject.activeSelf;
		toDeck.gameObject.SetActive(true);
		((GDFR_Deck_Script)toDeck).AddCardInstant(this);
		toDeck.gameObject.SetActive(deckActive);
	}

    public void MoveCardTestA()
	{
		//GameObject fromGO = GameObject.FindWithTag("from");
		GameObject toGo = GameObject.FindWithTag("to");
		MoveCard(toGo.transform,1f);
	}

	public void MoveCardTestB()
	{
		//GameObject fromGO = GameObject.FindWithTag("from");
		GameObject toGo = GameObject.FindWithTag("from");
		MoveCard(toGo.transform,1f);
	}

	public void bump()
	{
		cardBumpTweener.ResetToBeginning();
		cardBumpTweener.PlayForward();
	}

	void setZdepth(int z)
	{
		z +=_zDepthOffset;

		for(int w=0;w<widgetList.Length;w++)
		{
			widgetList[w].depth = widgetDefaultDepth[w] + z;
		}
		

	}

	public void symbolMatchEffect()
	{
		playTweenGroup(1);
	}

	public void playTweenGroup(int index)
	{
		foreach(UITweener t in tweenerList)
		{
			if(t.tweenGroup==index)
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

	IEnumerator CO_CardSparkleOverTime(float duration)
	{
		cardSparkle = true;
		yield return new WaitForSeconds(duration);
		cardSparkle = false;
	}

	public void PlayStarsEffect()
	{
		starsScript.enabled  = true;
	}
}


