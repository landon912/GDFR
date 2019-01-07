using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int Id = -1;

    private bool mCardSparkle;
    private bool mIsFront = true;

    private UITweener[] mTweenerList;
    private Canvas mCanvas;
    private Race mCurrentRace;
    private string mNameSound = "Not Set";

    public Deck parentDeck = null;
    public TweenScale cardBumpTweener = null;
    public TweenRotation cardFlipTweenerA = null;
    public TweenRotation cardFlipTweenerB = null;
    public TweenTransform cardLabelMoveOnFlipTweener = null;
    public TweenTransform cardMoveTweener = null;
    public TweenScale cardScaleTweener = null;

    public nEmitter cardSparkleEmitter = null;
    public RectTransform defaultLabelPosition;
    public TextMeshProUGUI fadingFlipText;
    public bool fairyStarBorder = false;
    public string fairyText = "Fairy Test";
    public bool goblinStarBorder = false;

    public string goblinText = "Goblin Test";
    public GameObject labelPositionOnFlip;

    public Rhyme fairyRhyme;
    public Rhyme goblinRhyme;
    public Symbol fairySymbol;
    public Symbol goblinSymbol;

    public string fairySpriteName = "";
    public string goblinSpriteName = "";
    public string moonSpriteName = "SYMBOL_Moon";
    public string mushroomSpriteName = "SYMBOL_Mushroom";
    public string sunSpriteName = "SYMBOL_Sun";
    public string frogSpriteName = "SYMBOL_Frog";

    public RectTransform flipTransform = null;
    public Image shadowSprite;

    public Image sprite;
    public bool starBorder = false;
    public CardStars starsScript;
    public Image symbolGlowSprite;
    public Image symbolSprite;
    public TextMeshProUGUI text;

    public const int NUMBERS_FONT_SIZE = 60;
    public const int NUMBERS_END_LABEL_X_POSITION = 90;
    public const int REGULAR_FONT_SIZE = 36;
    public const int REGULAR_END_LABEL_X_POSITION = 0;

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

    public SymbolGroup CurrentSymbolGroup
    {
        get
        {
            return CurrentSymbol == Symbol.Frog || CurrentSymbol == Symbol.Mushroom ?
                SymbolGroup.FrogMushroom :
                SymbolGroup.SunMoon;
        }
    }

    public Rhyme CurrentRhyme { get; private set; }

    public string NameSound { get { return mNameSound; } }

    private RectTransform mRectTransform;
    public RectTransform LocalRectTransform
    {
        get
        {
            if (mRectTransform == null){ mRectTransform = GetComponent<RectTransform>(); }
            return mRectTransform;
        } 
    }

    private UnityAction localCardSelectedCallback;

    private int _depth;
    public int Depth
    {
        set
        {
            _depth = value;
            UpdateDepth();
        }
        get
        {
            return _depth;
        }
    }

    private int _depthOffset;
    public int DeckDepthOffset
    {
        set
        {
            _depthOffset = value;
            UpdateDepth();
        }
        get{ return _depthOffset; }
    }

    public bool cardSparkle
    {
        set
        {
            mCardSparkle = value;
            cardSparkleEmitter.enabled = mCardSparkle;
        }
        get { return mCardSparkle; }
    }

    private void Start()
    {
        ChangeRace(mCurrentRace);
    }

    private void Awake()
    {
        LocalRectTransform.localScale = Vector3.one;
        mTweenerList = GetComponentsInChildren<UITweener>();
        cardSparkle = mCardSparkle;

        mCanvas = gameObject.AddComponent<Canvas>();
        mCanvas.overrideSorting = true;

        gameObject.AddComponent<GraphicRaycaster>().ignoreReversedGraphics = false;

        localCardSelectedCallback += CardSelectedReplicator;
        GetComponent<Button>().onClick.AddListener(localCardSelectedCallback);

        text.GetComponent<RectTransform>().anchoredPosition = defaultLabelPosition.anchoredPosition;
    }

    private void CardSelectedReplicator()
    {
        UI_Event_Receiver.TriggerCardSelectedEvent(this);
    }

    public IEnumerator Flip(bool wasFromStar)
    {
        EventReceiver.TriggerCardFlipEvent(this, wasFromStar);
        yield return StartCoroutine(AnimateFlip(wasFromStar));
    }

    public IEnumerator AnimateFlip(bool wasFromStar)
    {
        if (!wasFromStar)
        {
            fadingFlipText.text = text.text;
            fadingFlipText.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
            MoveLabel(labelPositionOnFlip.GetComponent<RectTransform>(), 1f);
        }

        if (mIsFront)
            cardFlipTweenerA.PlayForward();
        else
            cardFlipTweenerA.PlayReverse();

        yield return new WaitForSeconds(cardFlipTweenerA.duration/2.0f);

        text.gameObject.SetActive(true);

        mIsFront = !mIsFront;

        if (mCurrentRace == Race.Fairy)
        {
            CurrentRace = Race.Goblin;
            if (Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers)
            {
                text.text = ((int) CurrentRhyme).ToString();
                SetTextSettings(true);
            }
            else
            {
                text.text = goblinText;
                SetTextSettings(false);
            }
            SetSymbol(goblinSymbol);
        }
        else
        {
            CurrentRace = Race.Fairy;
            text.text = Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers
                ? ((int) CurrentRhyme).ToString()
                : fairyText;
            SetSymbol(fairySymbol);
        }

        flipTransform.localScale = mIsFront ? Vector3.one : new Vector3(-1, 1, 1);

        yield return new WaitForSeconds(0.5f);

        fadingFlipText.gameObject.SetActive(false);
    }

    private void SetSymbol(Symbol symbol)
    {
        switch (symbol)
        {
            case Symbol.Sun:
                symbolSprite.sprite = GameContoller.Instance.spriteSwapper.spriteDict[sunSpriteName];
                break;
            case Symbol.Moon:
                symbolSprite.sprite = GameContoller.Instance.spriteSwapper.spriteDict[moonSpriteName];
                break;
            case Symbol.Mushroom:
                symbolSprite.sprite = GameContoller.Instance.spriteSwapper.spriteDict[mushroomSpriteName];
                break;
            case Symbol.Frog:
                symbolSprite.sprite = GameContoller.Instance.spriteSwapper.spriteDict[frogSpriteName];
                break;
        }
        symbolGlowSprite.sprite = GameContoller.Instance.spriteSwapper.spriteDict[symbolSprite.sprite.name + "_Glow"];
    }

    public IEnumerator AnimateDrawCard(Deck toDeck, float duration)
    {
        parentDeck.RemoveCard(this);
        toDeck.AddCard(this);
        yield return new WaitForSeconds(duration);
    }

    public void ChangeRace(Race race)
    {
        switch (race)
        {
            case Race.Fairy:
                sprite.sprite = GameContoller.Instance.spriteSwapper.spriteDict[fairySpriteName];
                mCurrentRace = Race.Fairy;
                CurrentRhyme = fairyRhyme;
                CurrentSymbol = fairySymbol;
                if (Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers)
                {
                    text.text = ((int) CurrentRhyme).ToString();
                    SetTextSettings(true);
                }
                else
                {
                    text.text = fairyText;
                    SetTextSettings(false);
                }

                //Debug.Log ("HIT  Race Set To Fairy");
                break;
            case Race.Goblin:
                sprite.sprite = GameContoller.Instance.spriteSwapper.spriteDict[goblinSpriteName];
                mCurrentRace = Race.Goblin;
                CurrentRhyme = goblinRhyme;
                CurrentSymbol = goblinSymbol;
                if (Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers)
                {
                    text.text = ((int) CurrentRhyme).ToString();
                    SetTextSettings(true);
                }
                else
                {
                    text.text = goblinText;
                    SetTextSettings(false);
                }

                //Debug.Log ("Race Set To goblin");
                break;
        }
        SetSymbol(CurrentSymbol);
        if (StarsShowing)
        {
            starsScript.Show();
            //PlayStarsEffect();
        }
        else
        {
            starsScript.Hide();
        }
    }

    private void SetTextSettings(bool isNumbersVariant)
    {
        if (isNumbersVariant)
        {
            text.fontSize = NUMBERS_FONT_SIZE;
            text.alignment = TextAlignmentOptions.Right;
            fadingFlipText.fontSize = NUMBERS_FONT_SIZE;
            fadingFlipText.alignment = TextAlignmentOptions.Right;

            RectTransform flipTrans = labelPositionOnFlip.GetComponent<RectTransform>();
            flipTrans.anchoredPosition3D = new Vector3(NUMBERS_END_LABEL_X_POSITION, flipTrans.anchoredPosition3D.y, flipTrans.anchoredPosition3D.z);
        }
        else
        {
            text.fontSize = REGULAR_FONT_SIZE;
            fadingFlipText.fontSize = REGULAR_FONT_SIZE;

            RectTransform flipTrans = labelPositionOnFlip.GetComponent<RectTransform>();
            flipTrans.anchoredPosition3D = new Vector3(REGULAR_END_LABEL_X_POSITION, flipTrans.anchoredPosition3D.y, flipTrans.anchoredPosition3D.z);
        }
    }

    public void MoveCard(RectTransform to, float duration)
    {
        Move(to, duration, cardMoveTweener, LocalRectTransform);
    }

    public void MoveLabel(RectTransform to, float duration)
    {
        Move(to, duration, cardLabelMoveOnFlipTweener, defaultLabelPosition);
    }

    public void Move(RectTransform to, float duration, TweenTransform tween, RectTransform from)
    {
        var toParent = to.parent;

        tween.from = from;
        tween.to = to;
        tween.duration = duration;

        tween.ResetToBeginning();
        tween.Play(true);
    }

    public void MoveToNewDeck(Deck toDeck, RectTransform parent = null)
    {
        var deckActive = toDeck.gameObject.activeSelf;
        toDeck.gameObject.SetActive(true);
        toDeck.AddCard(this);
        toDeck.gameObject.SetActive(deckActive);
    }

    public void MoveToNewDeckInstant(Deck toDeck, RectTransform parent = null)
    {
        var deckActive = toDeck.gameObject.activeSelf;
        toDeck.gameObject.SetActive(true);
        toDeck.AddCardInstant(this);
        toDeck.gameObject.SetActive(deckActive);
    }

    public void Bump()
    {
        cardBumpTweener.ResetToBeginning();
        cardBumpTweener.PlayForward();
    }

    private void UpdateDepth()
    {
        mCanvas.overrideSorting = true;
        mCanvas.sortingOrder = Depth + DeckDepthOffset;
    }

    public void SymbolMatchEffect()
    {
        const int ALPHA_TIME = 2;
        AnimationCurve curve = symbolGlowSprite.GetComponent<TweenCurve>().tweenCurves[0];
        LeanTween.alpha(symbolGlowSprite.rectTransform, 1, ALPHA_TIME).setEase(curve);
    }

    //public void PlayTweenGroup(int index)
    //{
    //    foreach (var t in mTweenerList)
    //    {
    //        if (t.tweenGroup == index)
    //        {
    //            t.ResetToBeginning();
    //            t.PlayForward();
    //        }
    //    }
    //}

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
        starsScript.Play();
    }

    public override string ToString()
    {
        string name = CurrentRace == Race.Fairy ? fairyText : goblinText;
        return name + ", " + CurrentSymbol + ", " + CurrentRace + ", " + CurrentRhyme;
    }
}