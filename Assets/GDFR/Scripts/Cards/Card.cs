using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GDFR
{
    public class Card : MonoBehaviour
    {
        public const float BUMP_TIME_SECONDS = 0.3f;
        public const int FLIP_TIME_SECONDS = 1;
        public const float FLIP_TEXT_TWEEN_SCALE = 1.3f;

        public const int NUMBERS_FONT_SIZE = 60;
        public const int NUMBERS_END_LABEL_X_POSITION = 90;
        public const int REGULAR_FONT_SIZE = 36;
        public const int REGULAR_END_LABEL_X_POSITION = 0;

        private const string MOON_SPRITE_NAME = "SYMBOL_Moon";
        private const string MUSHROOM_SPRITE_NAME = "SYMBOL_Mushroom";
        private const string SUN_SPRITE_NAME = "SYMBOL_Sun";
        private const string FROG_SPRITE_NAME = "SYMBOL_Frog";

        public int Id = -1;

        public Deck ParentDeck = null;
        public Image CardSprite;
        public TextMeshProUGUI MainText;
        public Image SymbolSprite;
        public Image SymbolGlowSprite;

        public bool FairyStarBorder = false;
        public bool GoblinStarBorder = false;
        public bool StarBorder = false;
        public CardStars StarsScript;

        public string FairyText = "Fairy Test";
        public string GoblinText = "Goblin Test";

        public Rhyme FairyRhyme;
        public Rhyme GoblinRhyme;
        public Symbol FairySymbol;
        public Symbol GoblinSymbol;

        public string FairySpriteName = "";
        public string GoblinSpriteName = "";

        private bool mIsFront = true;
        private Race mCurrentRace;

#pragma warning disable CS0649 //never assigned to
        private Canvas mCanvas;
        [SerializeField] private RectTransform mMoveRect;
        [SerializeField] private TweenCurve mMoveEaseCurve;
        [SerializeField] private RectTransform mScaleRect;
        [SerializeField] private TweenCurve mScaleEaseCurve;
        [SerializeField] private RectTransform mFlipRect;
        [SerializeField] private TweenCurve mFlipEaseCurve;
        [SerializeField] private RectTransform mFlippingTextRect;
        [SerializeField] private TweenCurve mFlippingTextEaseCurves;
        [SerializeField] private TextMeshProUGUI mFlippingText;
        [SerializeField] private nEmitter mCardSparkleEmitter;
        [SerializeField] private RectTransform mEndLocationForFlippingText;
#pragma warning restore CS0649

        private UnityAction mLocalCardSelectedCallback;

        public bool AreStarsShowing
        {
            get
            {
                switch (mCurrentRace)
                {
                    case Race.Goblin when GoblinStarBorder:
                        return true;
                    case Race.Fairy when FairyStarBorder:
                        return true;
                }

                return false;
            }
        }

        public Race CurrentRace
        {
            set
            {
                if (value == mCurrentRace) return;
                ChangeRace(value);
            }
            get => mCurrentRace;
        }

        public Symbol CurrentSymbol { get; private set; }

        public SymbolGroup CurrentSymbolGroup
        {
            get
            {
                return CurrentSymbol == Symbol.Frog || CurrentSymbol == Symbol.Mushroom
                    ? SymbolGroup.FrogMushroom
                    : SymbolGroup.SunMoon;
            }
        }

        public Rhyme CurrentRhyme { get; private set; }

        public string NameSound { get; } = "Not Set";

        private RectTransform mRectTransform;

        public RectTransform LocalRectTransform
        {
            get
            {
                if (mRectTransform == null) { mRectTransform = GetComponent<RectTransform>(); }

                return mRectTransform;
            }
        }

        private int mDepth;

        public int Depth
        {
            set
            {
                mDepth = value;
                UpdateDepth();
            }
            get => mDepth;
        }

        private int mDepthOffset;

        public int DeckDepthOffset
        {
            set
            {
                mDepthOffset = value;
                UpdateDepth();
            }
            get => mDepthOffset;
        }

        private bool mCardSparkleOn;

        public bool CardSparkleOn
        {
            set
            {
                mCardSparkleOn = value;
                mCardSparkleEmitter.enabled = mCardSparkleOn;
            }
            get => mCardSparkleOn;
        }

        private void Start()
        {
            ChangeRace(mCurrentRace);
        }

        private void Awake()
        {
            LocalRectTransform.localScale = Vector3.one;
            CardSparkleOn = mCardSparkleOn;

            mCanvas = gameObject.AddComponent<Canvas>();
            mCanvas.overrideSorting = true;

            gameObject.AddComponent<GraphicRaycaster>().ignoreReversedGraphics = false;

            mLocalCardSelectedCallback += CardSelectedReplicator;
            GetComponent<Button>().onClick.AddListener(mLocalCardSelectedCallback);
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
                TriggerFlippingText();
            }

            LeanTween.rotateAroundLocal(mFlipRect.gameObject, Vector3.up, 180f, FLIP_TIME_SECONDS)
                .setEase(mFlipEaseCurve.tweenCurves[0]);

            yield return new WaitForSeconds(FLIP_TIME_SECONDS / 2.0f);

            MainText.gameObject.SetActive(true);

            mIsFront = !mIsFront;

            if (mCurrentRace == Race.Fairy)
            {
                CurrentRace = Race.Goblin;
                if (Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers)
                {
                    MainText.text = ((int) CurrentRhyme).ToString();
                    SetTextSettings(true);
                }
                else
                {
                    MainText.text = GoblinText;
                    SetTextSettings(false);
                }

                SetSymbol(GoblinSymbol);
            }
            else
            {
                CurrentRace = Race.Fairy;
                MainText.text = Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers
                    ? ((int) CurrentRhyme).ToString()
                    : FairyText;
                SetSymbol(FairySymbol);
            }

            mFlipRect.localScale = mIsFront ? Vector3.one : new Vector3(-1, 1, 1);

            yield return new WaitForSeconds(FLIP_TIME_SECONDS / 2.0f);

            mFlippingText.gameObject.SetActive(false);
        }

        private void TriggerFlippingText()
        {
            //enable/disable
            mFlippingText.text = MainText.text;
            mFlippingText.gameObject.SetActive(true);
            MainText.gameObject.SetActive(false);

            //reset state
            mFlippingTextRect.anchoredPosition3D = MainText.rectTransform.anchoredPosition3D;
            mFlippingTextRect.localScale = Vector3.one;

            //tween
            Move(mFlippingTextRect, mEndLocationForFlippingText.anchoredPosition3D, FLIP_TIME_SECONDS,
                mFlippingTextEaseCurves.tweenCurves[0]);
            LeanTween.scale(mFlippingTextRect,
                new Vector3(FLIP_TEXT_TWEEN_SCALE, FLIP_TEXT_TWEEN_SCALE, FLIP_TEXT_TWEEN_SCALE), FLIP_TIME_SECONDS);
        }

        private void SetSymbol(Symbol symbol)
        {
            switch (symbol)
            {
                case Symbol.Sun:
                    SymbolSprite.sprite = GameController.Instance.spriteSwapper.spriteDict[SUN_SPRITE_NAME];
                    break;
                case Symbol.Moon:
                    SymbolSprite.sprite = GameController.Instance.spriteSwapper.spriteDict[MOON_SPRITE_NAME];
                    break;
                case Symbol.Mushroom:
                    SymbolSprite.sprite = GameController.Instance.spriteSwapper.spriteDict[MUSHROOM_SPRITE_NAME];
                    break;
                case Symbol.Frog:
                    SymbolSprite.sprite = GameController.Instance.spriteSwapper.spriteDict[FROG_SPRITE_NAME];
                    break;
            }

            SymbolGlowSprite.sprite =
                GameController.Instance.spriteSwapper.spriteDict[SymbolSprite.sprite.name + "_Glow"];
        }

        public IEnumerator AnimateDrawCard(Deck toDeck, float duration)
        {
            ParentDeck.RemoveCard(this);
            toDeck.AddCard(this);
            yield return new WaitForSeconds(duration);
        }

        public void ChangeRace(Race race)
        {
            switch (race)
            {
                case Race.Fairy:
                    CardSprite.sprite = GameController.Instance.spriteSwapper.spriteDict[FairySpriteName];
                    mCurrentRace = Race.Fairy;
                    CurrentRhyme = FairyRhyme;
                    CurrentSymbol = FairySymbol;
                    if (Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers)
                    {
                        MainText.text = ((int) CurrentRhyme).ToString();
                        SetTextSettings(true);
                    }
                    else
                    {
                        MainText.text = FairyText;
                        SetTextSettings(false);
                    }

                    break;
                case Race.Goblin:
                    CardSprite.sprite = GameController.Instance.spriteSwapper.spriteDict[GoblinSpriteName];
                    mCurrentRace = Race.Goblin;
                    CurrentRhyme = GoblinRhyme;
                    CurrentSymbol = GoblinSymbol;
                    if (Toolbox.Instance.gameSettings.CardVariant == GameSettings.CardVariantType.Numbers)
                    {
                        MainText.text = ((int) CurrentRhyme).ToString();
                        SetTextSettings(true);
                    }
                    else
                    {
                        MainText.text = GoblinText;
                        SetTextSettings(false);
                    }

                    break;
            }

            SetSymbol(CurrentSymbol);
            if (AreStarsShowing)
            {
                StarsScript.Show();
                //PlayStarsEffect();
            }
            else
            {
                StarsScript.Hide();
            }
        }

        private void SetTextSettings(bool isNumbersVariant)
        {
            if (isNumbersVariant)
            {
                MainText.fontSize = NUMBERS_FONT_SIZE;
                MainText.alignment = TextAlignmentOptions.Right;
                mFlippingText.fontSize = NUMBERS_FONT_SIZE;
                mFlippingText.alignment = TextAlignmentOptions.Right;

                RectTransform flipTrans = mEndLocationForFlippingText.GetComponent<RectTransform>();
                flipTrans.anchoredPosition3D = new Vector3(NUMBERS_END_LABEL_X_POSITION, flipTrans.anchoredPosition3D.y,
                    flipTrans.anchoredPosition3D.z);
            }
            else
            {
                MainText.fontSize = REGULAR_FONT_SIZE;
                mFlippingText.fontSize = REGULAR_FONT_SIZE;

                RectTransform flipTrans = mEndLocationForFlippingText.GetComponent<RectTransform>();
                flipTrans.anchoredPosition3D = new Vector3(REGULAR_END_LABEL_X_POSITION, flipTrans.anchoredPosition3D.y,
                    flipTrans.anchoredPosition3D.z);
            }
        }

        public void MoveCard(Vector3 to, float duration)
        {
            Move(mMoveRect, to, duration, mMoveEaseCurve.tweenCurves[0]);
        }

        public void Move(RectTransform rect, Vector3 to, float duration, AnimationCurve easement)
        {
            LeanTween.move(rect, to, duration).setEase(easement);
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
            LeanTween.scale(mScaleRect, new Vector3(1.2f, 1.2f, 1.2f), BUMP_TIME_SECONDS)
                .setEase(mScaleEaseCurve.tweenCurves[0]);
            //cardBumpTweener.ResetToBeginning();
            //cardBumpTweener.PlayForward();
        }

        private void UpdateDepth()
        {
            mCanvas.overrideSorting = true;
            mCanvas.sortingOrder = Depth + DeckDepthOffset;
        }

        public void SymbolMatchEffect()
        {
            const int ALPHA_TIME = 2;
            AnimationCurve curve = SymbolGlowSprite.GetComponent<TweenCurve>().tweenCurves[0];
            LeanTween.alpha(SymbolGlowSprite.rectTransform, 1, ALPHA_TIME).setEase(curve);
        }

        public void CardSparkleOverTime(float duration)
        {
            StartCoroutine(CO_CardSparkleOverTime(duration));
        }

        private IEnumerator CO_CardSparkleOverTime(float duration)
        {
            CardSparkleOn = true;
            yield return new WaitForSeconds(duration);
            CardSparkleOn = false;
        }

        public void PlayStarsEffect()
        {
            StarsScript.Play();
        }

        public override string ToString()
        {
            string name = CurrentRace == Race.Fairy ? FairyText : GoblinText;
            return name + ", " + CurrentSymbol + ", " + CurrentRace + ", " + CurrentRhyme;
        }
    }
}