using UnityEngine;
using System;

public class GameSettingUIEvents : MonoBehaviour {

	public UnityEngine.UI.Text playerCountLabel = null;
    public UnityEngine.UI.Button numberOfPlayersAdd = null;
    public UnityEngine.UI.Button numberOfPlayersRemove = null;
    public UnityEngine.UI.Dropdown difficultyDropDown = null;
	public UnityEngine.UI.Dropdown cardVariantDropDown = null;
	public UnityEngine.UI.Dropdown rulesVariantDropDown = null;
    public GameObject playerControl = null;
    public GameObject playerItemControl = null;

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
        // Set the current numberOfPlayers
        playerCountLabel.text = Toolbox.Instance.gameSettings.numberOfPlayers.ToString();

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

    void Destroy()
    {

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
        GameObject newPlayerPanel = Instantiate(playerItemControl, new Vector3(0, -(32.8f + (68.6f * idx)), 0), Quaternion.identity);

        // set the player profile index on the UI component, so it can modify this player settings
        PlayerProfile_UI playerUI = newPlayerPanel.GetComponent<PlayerProfile_UI>();

        playerUI.ProfileIndex = idx;
        playerUI.aiToggle.gameObject.SetActive(canBeAI);

        playerUI.PlayToggleSound = false;
        playerUI.aiToggle.isOn = isAI;
        playerUI.PlayToggleSound = true;

        if (isAI)
        {
            SelectDefaultName(playerUI);
        }
        else
        {
            Toolbox.Instance.playerProfiles[playerUI.ProfileIndex].name = "Player " + (idx + 1);
        }

        newPlayerPanel.transform.SetParent(playerControl.transform, false);
        newPlayerPanel.name = "player" + (idx+1);
    }

    public void SelectDefaultName(PlayerProfile_UI playerUI)
    {
        //select AI name from list
        playerUI.nameField.text = "AI Name " + (playerUI.ProfileIndex+1);
        playerUI.NameChangeStringToIgnore = playerUI.nameField.text;
        playerUI.HasDefaultName = true;

        playerUI.OnNameChanged(playerUI.nameField.text);
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
        else if ((currentNumber > count) && (count > 0)) // removing and > 0
        {
            int delta = Mathf.Abs(count - currentNumber);
            for(int i = delta; i > 0; i--)
            {
                Destroy(GameObject.Find("player" + (count + i)));
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
        Debug.Log("Difficulty Set to " + Toolbox.Instance.gameSettings.difficultyLevel);

    }

    void OnCardVariantChanged(int listValue)
	{
        Toolbox.Instance.gameSettings.cardVariant = (GameSettings.CardVariant)listValue;
        Debug.Log("CardVariant Set to " + Toolbox.Instance.gameSettings.cardVariant);
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
        Debug.Log("RulesVariant Set to " + Toolbox.Instance.gameSettings.rulesVariant);
    }	
}