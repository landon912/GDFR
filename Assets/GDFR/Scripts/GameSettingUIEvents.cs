using UnityEngine;
using System.Collections;
using System;

public class GameSettingUIEvents : MonoBehaviour {

	public UnityEngine.UI.Text playerCountLabel = null;
    public UnityEngine.UI.Button numberOfPlayersAdd = null;
    public UnityEngine.UI.Button numberOfPlayersRemove = null;
    public UnityEngine.UI.Dropdown difficultyDropDown = null;
	public UnityEngine.UI.Dropdown cardVariantDropDown = null;
	public UnityEngine.UI.Dropdown rulesVariantDropDown = null;
    public UnityEngine.GameObject playerControl = null;
    public UnityEngine.GameObject playerItemControl = null;

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
            bool canBeAI = true;
            //if (idx == 0) canBeAI = false;
            createNewPlayerProfile(idx, canBeAI);
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

    void createNewPlayerProfile(int idx)
    {
        this.createNewPlayerProfile(idx, true);
    }

    void createNewPlayerProfile(int idx, bool canBeAI)
    {
        // add a new player panel
        UnityEngine.GameObject newPlayerPanel = (GameObject)Instantiate(playerItemControl, new Vector3(0, -(32.8f + (68.6f * idx)), 0), Quaternion.identity);

        // set the player profile index on the UI component, so it can modify this player settings
        newPlayerPanel.GetComponent<PlayerProfile_UI>().ProfileIndex = idx;
        newPlayerPanel.GetComponent<PlayerProfile_UI>().aiToggle.gameObject.SetActive(canBeAI);

        newPlayerPanel.transform.SetParent(playerControl.transform, false);
        newPlayerPanel.name = "player" + (idx+1).ToString();
    }

    void ValidateAddAndRemoveButtons()
    {
        if (Toolbox.Instance.gameSettings.numberOfPlayers == Toolbox.MAX_NUMBER_PLAYERS)
        {
            numberOfPlayersAdd.interactable = false;
        }
        else
        {
            numberOfPlayersAdd.interactable = true;
        }

        if (Toolbox.Instance.gameSettings.numberOfPlayers == 1)
        {
            numberOfPlayersRemove.interactable = false;
        }
        else
        {
            numberOfPlayersRemove.interactable = true;
        }
    }

	void OnPlayerCountChanged(int count)
	{
        int currentNumber = Int32.Parse(playerCountLabel.text);

        // adding and < MAX
        if ((currentNumber < count) && (count <= Toolbox.MAX_NUMBER_PLAYERS))
        {
            createNewPlayerProfile(count - 1);
        }
        else if ((currentNumber > count) && (count > 0)) // removing and > 0
        {
            Destroy(GameObject.Find("player" + (count+1).ToString()));
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
