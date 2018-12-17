using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class AIData
{
    public List<string> names = new List<string>(2);
    public int avatarID;
}

public class GameSettingUIEvents : MonoBehaviour
{
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
    public List<PlayerProfile_UI> playerProfiles;

    public static int AI_PROFILE_INDEX = 11;

    void OnEnable()
    {
        difficultyDropDown.onValueChanged.AddListener(OnDifficultyChanged);
        cardVariantDropDown.onValueChanged.AddListener(OnCardVariantChanged);
        rulesVariantDropDown.onValueChanged.AddListener(OnRulesVariantChanged);

        numberOfPlayersAdd.onClick.AddListener(OnClickAddPlayer);
        numberOfPlayersRemove.onClick.AddListener(OnClickRemovePlayer);
    }

    // Use this for initialization
    void Start()
    {
        LoadXMLData();

        if (NetworkServer.active || NetworkClient.active)
        {
            RegisterNetworkEvents();
            Toolbox.Instance.gameSettings.numberOfPlayers = GDFRNetworkManager.Instance.NumPlayers;
        }

        Setup();
    }

    private void Setup()
    {
        // Set the current NumberOfPlayers
        playerCountLabel.text = Toolbox.Instance.gameSettings.numberOfPlayers.ToString();

        playerProfiles = new List<PlayerProfile_UI>(4);

        // Create all player profiles after the first
        for (int idx = 0; idx < Toolbox.Instance.gameSettings.numberOfPlayers; idx++)
        {
            //CreateNewPlayerProfile(idx, true, idx != 0);
            CreateNewPlayerProfile(idx, false);
        }

        ValidateAddAndRemoveButtons();
        ValidateCombos(true);

        switch (Toolbox.Instance.gameSettings.RulesVariant)
        {
            case GameSettings.RulesVariantType.Solitaire:
            case GameSettings.RulesVariantType.Classic:
                rulesVariantDropDown.value = 0;
                break;
            case GameSettings.RulesVariantType.GoblinsRule:
            case GameSettings.RulesVariantType.UltimateSolitaire:
                rulesVariantDropDown.value = 1;
                break;
        }
        rulesVariantDropDown.RefreshShownValue();

        difficultyDropDown.value = (int)Toolbox.Instance.gameSettings.DifficultyLevel;
        difficultyDropDown.RefreshShownValue();

        cardVariantDropDown.value = (int)Toolbox.Instance.gameSettings.CardVariant;
        cardVariantDropDown.RefreshShownValue();
    }

    private void RegisterNetworkEvents()
    {
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.SetupPlayerCountChanged, NetOnPlayerCountChanged);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.SetupDifficultyChanged, NetOnDifficultyChanged);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.SetupCardVariantChanged, NetOnCardVariantChanged);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.SetupRulesVariantChanged, NetOnRulesVariantChanged);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.SetupHumanToggleChanged, OnServerChangeHumanToggle);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.SetupAvatarChanged, OnServerChangeAvatar);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.SetupNameChanged, OnServerChangeName);
    }

    private void OnDisable()
    {
        difficultyDropDown.onValueChanged.RemoveAllListeners();
        cardVariantDropDown.onValueChanged.RemoveAllListeners();
        rulesVariantDropDown.onValueChanged.RemoveAllListeners();

        numberOfPlayersAdd.onClick.RemoveAllListeners();
        numberOfPlayersRemove.onClick.RemoveAllListeners();

        GDFRNetworkManager.Instance?.localClient.UnregisterHandler(MsgIndexes.SetupPlayerCountChanged);
        GDFRNetworkManager.Instance?.localClient.UnregisterHandler(MsgIndexes.SetupDifficultyChanged);
        GDFRNetworkManager.Instance?.localClient.UnregisterHandler(MsgIndexes.SetupCardVariantChanged);
        GDFRNetworkManager.Instance?.localClient.UnregisterHandler(MsgIndexes.SetupRulesVariantChanged);
        GDFRNetworkManager.Instance?.localClient.UnregisterHandler(MsgIndexes.SetupHumanToggleChanged);
        GDFRNetworkManager.Instance?.localClient.UnregisterHandler(MsgIndexes.SetupAvatarChanged);
        GDFRNetworkManager.Instance?.localClient.UnregisterHandler(MsgIndexes.SetupNameChanged);
    }

    private void OnServerChangeHumanToggle(NetworkMessage message)
    {
        //only update if not host
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            PlayerToggleMessage toggleMess = message.ReadMessage<PlayerToggleMessage>();

            playerProfiles[toggleMess.idx].humanToggle.isOn = toggleMess.isHuman;
            playerProfiles[toggleMess.idx].aiToggle.isOn = !toggleMess.isHuman;
        }
    }

    private void OnServerChangeAvatar(NetworkMessage message)
    {
        //can be set from a client
        PlayerAvatarMessage avatarMess = message.ReadMessage<PlayerAvatarMessage>();

        playerProfiles[avatarMess.idx].ChangeAvatar(avatarMess.avatarId);
    }

    private void OnServerChangeName(NetworkMessage message)
    {
        //only update if not host
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            PlayerNameMessage nameMess = message.ReadMessage<PlayerNameMessage>();

            playerProfiles[nameMess.idx].nameField.text = nameMess.playerName;
        }
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

    //called from button callbacks
    void OnClickAddPlayer()
    {
        if (Toolbox.Instance.gameSettings.numberOfPlayers < Toolbox.MAX_NUMBER_PLAYERS)
        {
            ChangePlayerCount(Toolbox.Instance.gameSettings.numberOfPlayers + 1);
            Debug.Log("Player Number Set to " + Toolbox.Instance.gameSettings.numberOfPlayers);
        }
    }

    //called from button callbacks
    void OnClickRemovePlayer()
    {
        if (Toolbox.Instance.gameSettings.numberOfPlayers > 1)
        {
            ChangePlayerCount(Toolbox.Instance.gameSettings.numberOfPlayers-1);
            Debug.Log("Player Number Set to " + Toolbox.Instance.gameSettings.numberOfPlayers);
        }
    }

    PlayerProfile_UI CreateNewPlayerProfile(int idx, bool canBeAI = true, bool isAI = false)
    {
        // add a new player panel
        GameObject newPlayerPanel = Instantiate(playerItemControl, Vector3.zero, Quaternion.identity);

        newPlayerPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, -(68.6f * idx), 0);

        // set the player profile index on the UI component, so it can modify this player settings
        PlayerProfile_UI playerUI = newPlayerPanel.GetComponent<PlayerProfile_UI>();

        playerProfiles.Add(playerUI);

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

        if ((NetworkServer.active || NetworkClient.active) && idx < GDFRNetworkManager.Instance.NumPlayers)
        {
            playerUI.SetAsRepresentingClientId(idx);
        }

        return playerUI;
    }

    public void SelectDefaultProfile(PlayerProfile_UI playerUI)
    {
        //set public name
        playerUI.nameStatic.text = "A.I. Player";

        //set public avatarID to AI default (Needs to be before internal set)
        playerUI.ChangeAvatar(AI_PROFILE_INDEX);
    }

    public void SelectRealAIProfiles()
    {
        //remove ai profiles that use the same avatar as players
        foreach (PlayerProfile_UI playerUI in playerProfiles)
        {
            if (playerUI.humanToggle.isOn)
            {
                for (var i = 0; i < mAIProfiles.Count; i++)
                {
                    AIData aiProfile = mAIProfiles[i];
                    if (playerUI.CurrentAvatarID == aiProfile.avatarID)
                    {
                        mAIProfiles.Remove(aiProfile);
                    }
                }
            }
        }

        foreach (PlayerProfile_UI playerUI in playerProfiles)
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

        numberOfPlayersRemove.interactable = Toolbox.Instance.gameSettings.numberOfPlayers != 1 && Toolbox.Instance.gameSettings.numberOfPlayers != GDFRNetworkManager.Instance.NumPlayers;
    }

    void ChangePlayerCount(int count)
    {
        Toolbox.Instance.gameSettings.numberOfPlayers = count;

        GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.SetupPlayerCountChanged, new IntegerMessage(count));

        int currentNumber = Int32.Parse(playerCountLabel.text);
        playerCountLabel.text = count.ToString();

        // adding and < MAX
        if ((currentNumber < count) && (count <= Toolbox.MAX_NUMBER_PLAYERS))
        {
            int delta = Mathf.Abs(count - currentNumber);
            for (int i = delta; i > 0; i--)
            {
                CreateNewPlayerProfile(count - delta, true, true);
                delta--;
            }
        }
        else if (currentNumber > count && count > 0) // removing and > 0
        {
            int delta = Mathf.Abs(count - currentNumber);
            for (int i = delta; i > 0; i--)
            {
                GameObject playerUIObj = GameObject.Find("player" + (count + i));
                PlayerProfile_UI playerProfile = playerUIObj.GetComponent<PlayerProfile_UI>();

                playerProfiles.Remove(playerProfile);

                Destroy(playerUIObj);
                delta--;
            }
        }

        if ((NetworkServer.active && GDFRNetworkManager.Instance.IsLocalClientTheHost()) || (!NetworkServer.active && !NetworkClient.active))
        {
            ValidateAddAndRemoveButtons();
        }
        ValidateCombos();
    }

    private void NetOnPlayerCountChanged(NetworkMessage message)
    {
        //only update if not host
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            int count = message.ReadMessage<IntegerMessage>().value;
            ChangePlayerCount(count);
        }
    }

    void ValidateCombos(bool setup = false)
    {
        // Only 1 player, disable classic mode and open just the solitaire and ultimate solitaire
        if ((Toolbox.Instance.gameSettings.numberOfPlayers == 1) && (setup || rulesVariantDropDown.options[0].text.ToLower() == "classic"))
        {
            rulesVariantDropDown.options.Clear();
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Solitaire"));
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Ultimate Solitaire"));
            rulesVariantDropDown.RefreshShownValue();

            Toolbox.Instance.gameSettings.RulesVariant = GameSettings.RulesVariantType.Solitaire;

            //default to first option
            rulesVariantDropDown.value = 0;
        }
        else if ((Toolbox.Instance.gameSettings.numberOfPlayers > 1) && (setup || rulesVariantDropDown.options[0].text.ToLower() == "solitaire"))
        {
            rulesVariantDropDown.options.Clear();
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Classic"));
            rulesVariantDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Goblins Rule!"));
            rulesVariantDropDown.RefreshShownValue();

            Toolbox.Instance.gameSettings.RulesVariant = GameSettings.RulesVariantType.Classic;

            //default to first option
            rulesVariantDropDown.value = 0;
        }
    }

    //called from button callbacks
	void OnDifficultyChanged(int listValue)
	{
        Toolbox.Instance.gameSettings.DifficultyLevel = (GameSettings.Difficulty)listValue;

        GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.SetupDifficultyChanged, new IntegerMessage(listValue));
    }

    private void NetOnDifficultyChanged(NetworkMessage message)
    {
        //only update if not host
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            difficultyDropDown.value = message.ReadMessage<IntegerMessage>().value;
        }
    }

    void OnCardVariantChanged(int listValue)
	{
        Toolbox.Instance.gameSettings.CardVariant = (GameSettings.CardVariantType)listValue;

	    GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.SetupCardVariantChanged, new IntegerMessage(listValue));
    }

    private void NetOnCardVariantChanged(NetworkMessage message)
    {
        //only update if not host
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            cardVariantDropDown.value = message.ReadMessage<IntegerMessage>().value;
        }
    }

    void OnRulesVariantChanged(int listValue)
	{
        switch(rulesVariantDropDown.captionText.text.ToLower())
        {
            case "ultimate solitaire":
                Toolbox.Instance.gameSettings.RulesVariant = GameSettings.RulesVariantType.UltimateSolitaire;
                break;
            case "solitaire":
                Toolbox.Instance.gameSettings.RulesVariant = GameSettings.RulesVariantType.Solitaire;
                break;
            case "goblins rule!":
                Toolbox.Instance.gameSettings.RulesVariant = GameSettings.RulesVariantType.GoblinsRule;
                break;
            default:
            case "classic":
                Toolbox.Instance.gameSettings.RulesVariant = GameSettings.RulesVariantType.Classic;
                break;
        }

	    GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.SetupRulesVariantChanged, new IntegerMessage(listValue));
    }

    private void NetOnRulesVariantChanged(NetworkMessage message)
    {
        //only update if not host
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            rulesVariantDropDown.value = message.ReadMessage<IntegerMessage>().value;
        }
    }
}