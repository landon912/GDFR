using System;
using UnityEngine;

public class PlayerProfile_UI : MonoBehaviour {

    public UnityEngine.UI.InputField nameField = null;
    public UnityEngine.UI.Toggle humanToggle = null;
    public UnityEngine.UI.Toggle aiToggle = null;
    public UnityEngine.UI.Dropdown avatarDropdown = null;

    public bool HasDefaultName = false;
    public string NameChangeStringToIgnore = String.Empty;

    [HideInInspector]
    public AIData defaultProfileAssigned = null;

    int _profileIndex = 0;
    public int ProfileIndex
    {
        set
        {
            _profileIndex = value;

            // If this setting already exists
            if (Toolbox.Instance.playerProfiles[value].name != "")
            {
                nameField.text = Toolbox.Instance.playerProfiles[value].name;
            }

            if (Toolbox.Instance.playerProfiles[value].avatar > 0)
            {
                avatarDropdown.value = Toolbox.Instance.playerProfiles[value].avatar;
            }

            if (Toolbox.Instance.playerProfiles[value].type == PlayersProfile.Type.Human)
            {
                PlayToggleSound = false;

                humanToggle.isOn = true;
                aiToggle.isOn = false;

                PlayToggleSound = true;
            }
            else
            {
                PlayToggleSound = false;

                humanToggle.isOn = false;
                aiToggle.isOn = true;

                PlayToggleSound = true;
            }
        }
        get { return _profileIndex; }
    }

    public bool PlayToggleSound = true;

    // Registering Events
    void OnEnable()
    {
        nameField.onEndEdit.AddListener(OnNameChanged);
        humanToggle.onValueChanged.AddListener(OnHumanToggle);
        avatarDropdown.onValueChanged.AddListener(OnAvatarChanged);
    }

    // De-registering events
    void OnDisable()
    {
        nameField.onEndEdit.RemoveAllListeners();
        humanToggle.onValueChanged.RemoveAllListeners();
        avatarDropdown.onValueChanged.RemoveAllListeners();
    }

    // Events
    public void OnNameChanged(string value)
    {
        if (value == String.Empty)
        {
            HasDefaultName = true;
            Toolbox.Instance.playerProfiles[ProfileIndex].name = "Player " + (ProfileIndex + 1);
            Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " + Toolbox.Instance.playerProfiles[ProfileIndex].name);

            return;
        }

        Toolbox.Instance.playerProfiles[ProfileIndex].name = value;
        Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " + Toolbox.Instance.playerProfiles[ProfileIndex].name);

        //dont reset default name flag b/c we are assigning the default name
        if (NameChangeStringToIgnore != value)
        {
            HasDefaultName = false;
        }
    }

    public void OnHumanToggle(bool value)
    {
        if (value)
        {
            Toolbox.Instance.playerProfiles[ProfileIndex].type = PlayersProfile.Type.Human;
            if (HasDefaultName)
            {
                nameField.text = "";
                avatarDropdown.value = 0;

                FindObjectOfType<GameSettingUIEvents>().AddProfileBackToUnassignedList(defaultProfileAssigned);
                defaultProfileAssigned = null;

                Toolbox.Instance.playerProfiles[ProfileIndex].name = "Player " + (ProfileIndex + 1);
                Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " + Toolbox.Instance.playerProfiles[ProfileIndex].name);
            }
        }
        else
        {
            Toolbox.Instance.playerProfiles[ProfileIndex].type = PlayersProfile.Type.AI;
            if (nameField.text == string.Empty || nameField.text == "Player " + (ProfileIndex + 1))
            {
                FindObjectOfType<GameSettingUIEvents>().SelectDefaultProfile(this);
            }
        }

        //skip us manually changing the first profile to a player automatically
        if (PlayToggleSound)
        {
            EventReceiver.TriggerButtonPressedEvent();
        }

        Debug.Log("Player [ " + ProfileIndex + " ] TYPE set to " + Toolbox.Instance.playerProfiles[ProfileIndex].type);
    }

    void OnAvatarChanged(int listValue)
    {
        Toolbox.Instance.playerProfiles[ProfileIndex].avatar = listValue;

        Debug.Log("Player [ " + ProfileIndex + " ] AVATAR set to " + Toolbox.Instance.playerProfiles[this.ProfileIndex].avatar);
    }
}
