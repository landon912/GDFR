﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

public class AIData
{
    public List<string> names = new List<string>(2);
    public int avatarID;
}

public class GameSettingUIEvents : MonoBehaviour {

	public UnityEngine.UI.Text playerCountLabel = null;
    public UnityEngine.UI.Button numberOfPlayersAdd = null;
    public UnityEngine.UI.Button numberOfPlayersRemove = null;
    public UnityEngine.UI.Dropdown difficultyDropDown = null;
	public UnityEngine.UI.Dropdown cardVariantDropDown = null;
	public UnityEngine.UI.Dropdown rulesVariantDropDown = null;
    public GameObject playerControl = null;
    public GameObject playerItemControl = null;
    public TextAsset AIDataAsset;

    private List<AIData> mAIProfiles;
    private List<PlayerProfile_UI> mPlayerProfiles;
    private static int AI_PROFILE_INDEX = 11;

    void OnEnable()
	{
        difficultyDropDown.onValueChanged.AddListener(OnDifficultyChanged);
        cardVariantDropDown.onValueChanged.AddListener(OnCardVariantChanged);
        rulesVariantDropDown.onValueChanged.AddListener(OnRulesVariantChanged);

        numberOfPlayersAdd.onClick.AddListener(OnClickAddPlayer);
        numberOfPlayersRemove.onClick.AddListener(OnClickRemovePlayer);
    }

	void OnDisable()
	{

        difficultyDropDown.onValueChanged.RemoveAllListeners();
        cardVariantDropDown.onValueChanged.RemoveAllListeners();
        rulesVariantDropDown.onValueChanged.RemoveAllListeners();

        numberOfPlayersAdd.onClick.RemoveAllListeners();
        numberOfPlayersRemove.onClick.RemoveAllListeners();
    }

    // Use this for initialization
    void Start()
    {
        LoadXMLData();

        // Set the current numberOfPlayers
        playerCountLabel.text = Toolbox.Instance.gameSettings.numberOfPlayers.ToString();

        mPlayerProfiles = new List<PlayerProfile_UI>(4);

        // Create all player profiles after the first
        for (int idx = 0; idx < Toolbox.Instance.gameSettings.numberOfPlayers; idx++)
        {
            createNewPlayerProfile(idx, true, idx != 0);
        }

        ValidateAddAndRemoveButtons();
        ValidateCombos();

        switch(Toolbox.Instance.gameSettings.rulesVariant)
        {
            case GameSettings.RulesVariant.Solitaire:
            case GameSettings.RulesVariant.Classic:
                rulesVariantDropDown.value = 0;
                break;
            case GameSettings.RulesVariant.Goblins_Rule:
            case GameSettings.RulesVariant.Ultimate_Solitaire:
                rulesVariantDropDown.value = 1;
                break;
        }
        rulesVariantDropDown.RefreshShownValue();

        difficultyDropDown.value = (int)Toolbox.Instance.gameSettings.difficultyLevel;
        difficultyDropDown.RefreshShownValue();

        cardVariantDropDown.value = (int)Toolbox.Instance.gameSettings.cardVariant;
        cardVariantDropDown.RefreshShownValue();

        //OnPlayerCountChanged(Toolbox.Instance.gameSettings.numberOfPlayers);
    }

    void LoadXMLData()
    {
        XmlDocument aiDocument = new XmlDocument();
        aiDocument.LoadXml(AIDataAsset.text);

        XmlNode rootAINode = aiDocument.SelectSingleNode("Avatars");

        mAIProfiles = new List<AIData>(rootAINode.ChildNodes.Count * 2);

        foreach (XmlNode avatar in rootAINode.ChildNodes)
        {
            XmlNode idNode = avatar.SelectSingleNode("id");
            int avatarID = Int32.Parse(idNode.InnerText);

            XmlNode names = avatar.SelectSingleNode("names");

            AIData data = new AIData
            {
                avatarID = avatarID
            };

            foreach (XmlNode name in names.ChildNodes)
            {
                data.names.Add(name.InnerText);
            }

            mAIProfiles.Add(data);
        }
    }

    void OnClickAddPlayer()
    {
        if (Toolbox.Instance.gameSettings.numberOfPlayers < Toolbox.MAX_NUMBER_PLAYERS)
        {
            Toolbox.Instance.gameSettings.numberOfPlayers++;
            OnPlayerCountChanged(Toolbox.Instance.gameSettings.numberOfPlayers);
            Debug.Log("Player Number Set to " + Toolbox.Instance.gameSettings.numberOfPlayers);
        }
    }

    void OnClickRemovePlayer()
    {
        if (Toolbox.Instance.gameSettings.numberOfPlayers > 1)
        {
            Toolbox.Instance.gameSettings.numberOfPlayers--;
            OnPlayerCountChanged(Toolbox.Instance.gameSettings.numberOfPlayers);
            Debug.Log("Player Number Set to " + Toolbox.Instance.gameSettings.numberOfPlayers);
        }
    }

    void createNewPlayerProfile(int idx, bool canBeAI = true, bool isAI = false)
    {
        // add a new player panel
        GameObject newPlayerPanel = Instantiate(playerItemControl, Vector3.zero, Quaternion.identity);

        newPlayerPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, -(68.6f * idx), 0);

        // set the player profile index on the UI component, so it can modify this player settings
        PlayerProfile_UI playerUI = newPlayerPanel.GetComponent<PlayerProfile_UI>();

        mPlayerProfiles.Add(playerUI);

        playerUI.ProfileIndex = idx;
        playerUI.aiToggle.gameObject.SetActive(canBeAI);

        playerUI.PlayToggleSound = false;

        playerUI.aiToggle.isOn = isAI;
        if (!isAI)
        {
            playerUI.OnHumanToggle(!isAI);
        }

        playerUI.PlayToggleSound = true;

        newPlayerPanel.transform.SetParent(playerControl.transform, false);
        newPlayerPanel.name = "player" + (idx + 1);
    }

    public void SelectDefaultProfile(PlayerProfile_UI playerUI)
    {
        //set public name
        playerUI.nameStatic.text = "A.I. Player";

        //set public avatarID to AI default (Needs to be before internal set)
        playerUI.avatarDropdown.value = AI_PROFILE_INDEX;
        playerUI.avatarDropdown.interactable = false;
    }

    public void SelectRealAIProfiles()
    {
        //remove ai profiles that use the same avatar as players
        foreach (PlayerProfile_UI playerUI in mPlayerProfiles)
        {
            if (playerUI.humanToggle.isOn)
            {
                for (var i = 0; i < mAIProfiles.Count; i++)
                {
                    AIData aiProfile = mAIProfiles[i];
                    if (playerUI.avatarDropdown.value == aiProfile.avatarID)
                    {
                        mAIProfiles.Remove(aiProfile);
                    }
                }
            }
        }

        foreach (PlayerProfile_UI playerUI in mPlayerProfiles)
        {
            if (playerUI.aiToggle.isOn)
            {
                SelectRealAIProfile(playerUI);
            }
        }
    }

    private void SelectRealAIProfile(PlayerProfile_UI playerUI)
    {
        //get random AIProfile
        int index = UnityEngine.Random.Range(0, mAIProfiles.Count);
        AIData profileToUse = mAIProfiles[index];

        playerUI.defaultProfileAssigned = profileToUse;

        //set internal name
        Toolbox.Instance.playerProfiles[playerUI.ProfileIndex].name = profileToUse.names[UnityEngine.Random.Range(0, profileToUse.names.Count)];

        //now, set internal avatarID
        Toolbox.Instance.playerProfiles[playerUI.ProfileIndex].avatar = profileToUse.avatarID;

        //do not have the AI use the same avatar more than once
        mAIProfiles.Remove(profileToUse);
    }

    public void AddProfileBackToUnassignedList(AIData profile)
    {
        mAIProfiles.Add(profile);
    }

    void ValidateAddAndRemoveButtons()
    {
        numberOfPlayersAdd.interactable = Toolbox.Instance.gameSettings.numberOfPlayers != Toolbox.MAX_NUMBER_PLAYERS;

        numberOfPlayersRemove.interactable = Toolbox.Instance.gameSettings.numberOfPlayers != 1;
    }

	void OnPlayerCountChanged(int count)
	{
        int currentNumber = Int32.Parse(playerCountLabel.text);

        // adding and < MAX
        if ((currentNumber < count) && (count <= Toolbox.MAX_NUMBER_PLAYERS))
        {
            int delta = Mathf.Abs(count - currentNumber);
            for (int i = delta; i > 0; i--)
            {
                createNewPlayerProfile(count - delta, true, true);
                delta--;
            }
        }
        else if (currentNumber > count && count > 0) // removing and > 0
        {
            int delta = Mathf.Abs(count - currentNumber);
            for(int i = delta; i > 0; i--)
            {
                GameObject playerUIObj = GameObject.Find("player" + (count + i));
                PlayerProfile_UI playerProfile = playerUIObj.GetComponent<PlayerProfile_UI>();

                mPlayerProfiles.Remove(playerProfile);

                Destroy(playerUIObj);
                delta--;
            }
        }

        ValidateAddAndRemoveButtons();
        ValidateCombos();

        playerCountLabel.text = count.ToString();
    }

    void ValidateCombos()
    {
        // Only 1 player, disable classic mode and open just the solitaire and ultimate solitaire
        if ((Toolbox.Instance.gameSettings.numberOfPlayers == 1) && (rulesVariantDropDown.options[0].text.ToLower() == "classic"))
        {
            rulesVariantDropDown.options.Clear();
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Solitaire"));
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Ultimate Solitaire"));
            rulesVariantDropDown.RefreshShownValue();

            Toolbox.Instance.gameSettings.rulesVariant = GameSettings.RulesVariant.Solitaire;
        }
        else if ((Toolbox.Instance.gameSettings.numberOfPlayers > 1) && (rulesVariantDropDown.options[0].text.ToLower() == "solitaire"))
        {
            rulesVariantDropDown.options.Clear();
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Classic"));
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Goblins Rule!"));
            rulesVariantDropDown.RefreshShownValue();

            Toolbox.Instance.gameSettings.rulesVariant = GameSettings.RulesVariant.Classic;
        }
    }

	void OnDifficultyChanged(int listValue)
	{
        Toolbox.Instance.gameSettings.difficultyLevel = (GameSettings.Difficulty)listValue;
        //Debug.Log("Difficulty Set to " + Toolbox.Instance.gameSettings.difficultyLevel);
    }

    void OnCardVariantChanged(int listValue)
	{
        Toolbox.Instance.gameSettings.cardVariant = (GameSettings.CardVariant)listValue;
        //Debug.Log("CardVariant Set to " + Toolbox.Instance.gameSettings.cardVariant);
    }

	void OnRulesVariantChanged(int listValue)
	{
        switch(rulesVariantDropDown.captionText.text.ToLower())
        {
            case "ultimate solitaire":
                Toolbox.Instance.gameSettings.rulesVariant = GameSettings.RulesVariant.Ultimate_Solitaire;
                break;
            case "solitaire":
                Toolbox.Instance.gameSettings.rulesVariant = GameSettings.RulesVariant.Solitaire;
                break;
            case "goblins rule!":
                Toolbox.Instance.gameSettings.rulesVariant = GameSettings.RulesVariant.Goblins_Rule;
                break;
            default:
            case "classic":
                Toolbox.Instance.gameSettings.rulesVariant = GameSettings.RulesVariant.Classic;
                break;
        }
        //Debug.Log("RulesVariant Set to " + Toolbox.Instance.gameSettings.rulesVariant);
    }	
}