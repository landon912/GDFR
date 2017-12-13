using System;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlideData
{
    public string imageName;
    public string text;
}

public class HelpMenuController : MonoBehaviour
{
    public string slideXmlDataPath = "/GDFR/Data/Resources/HowToPlaySlidesData.xml";
    public Sprite[] mainImageSprites;
    public Image mainImage;
    public Text text;
    public Button backButton;
    public Button forwardButton;

    private int mCurrentSlideIndex;
    private SlideData[] mSlideData;

    void Start()
    {
        //hide main game to stop you from being able to click on things
        GameContoller gameContoller = FindObjectOfType<GameContoller>();
        if (gameContoller)
        {
            gameContoller.mainUICamera.enabled = false;
        }

        Time.timeScale = 0;

        LoadXMLFile();

        ApplySlide(mCurrentSlideIndex);
    }

    public void Advance()
    {
        ApplySlide(++mCurrentSlideIndex);
    }

    public void GoBack()
    {
        ApplySlide(--mCurrentSlideIndex);
    }

    public void Exit()
    {
        //bring back menu if within main menu
        MainMenuController menuContoller = FindObjectOfType<MainMenuController>();
        if (menuContoller)
        {
            menuContoller.mainAlphaTweener.PlayReverse();
        }

        //bring back main game to allow you to click on things
        GameContoller gameContoller = FindObjectOfType<GameContoller>();
        if (gameContoller)
        {
            gameContoller.mainUICamera.enabled = true;
        }

        Time.timeScale = 1;

        SceneManager.UnloadSceneAsync("Help_Additive");
    }

    private void LoadXMLFile()
    {
        XmlDocument slideDocument = new XmlDocument();
        slideDocument.Load(Application.dataPath + slideXmlDataPath);

        XmlNode rootSlideNode = slideDocument.SelectSingleNode("Slides");

        mSlideData = new SlideData[rootSlideNode.ChildNodes.Count];

        foreach (XmlNode slide in rootSlideNode.ChildNodes)
        {
            XmlNode indexNode = slide.SelectSingleNode("Index");
            int arrayIndex = Int32.Parse(indexNode.InnerText) - 1;

            mSlideData[arrayIndex] = new SlideData();

            XmlNode imageNode = slide.SelectSingleNode("ImagePath");
            mSlideData[arrayIndex].imageName = imageNode.InnerText;

            XmlNode textNode = slide.SelectSingleNode("Text");
            mSlideData[arrayIndex].text = textNode.InnerText;
        }
    }

    private void ApplySlide(int index)
    {
        for(int i = 0; i < mSlideData.Length; i++)
        {
            if (mainImageSprites[i].name == mSlideData[index].imageName)
            {
                mainImage.sprite = mainImageSprites[i];
                break;
            }

            if (i == mSlideData.Length - 1)
            {
                Debug.LogError("Could not find sprite for help menu");
            }
        }

        text.text = mSlideData[index].text;

        ValidateButtons();
    }

    private void ValidateButtons()
    {
        backButton.interactable = mCurrentSlideIndex != 0;
        forwardButton.interactable = mCurrentSlideIndex != mSlideData.Length-1;
    }
}