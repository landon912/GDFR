﻿using System;
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

    private int mCurrentSlideIndex = 0;
    private SlideData[] mSlideData;

    void Start()
    {
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
        SceneManager.UnloadSceneAsync("Help_Additive");
    }

    private void LoadXMLFile()
    {
        XmlDocument slideDocument = new XmlDocument();
        slideDocument.Load(Application.dataPath + slideXmlDataPath);

        XmlNode rootSlideNode = slideDocument.SelectSingleNode("Slides");

        Debug.Assert(rootSlideNode != null, "rootSlideNode != null");
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