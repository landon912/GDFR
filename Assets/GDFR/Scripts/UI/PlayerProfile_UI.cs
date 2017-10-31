using System;
using UnityEngine;

public class PlayerProfile_UI : MonoBehaviour {

    public UnityEngine.UI.InputField nameField = null;
    public UnityEngine.UI.Toggle humanToggle = null;
    public UnityEngine.UI.Toggle aiToggle = null;
    public UnityEngine.UI.Dropdown avatarDropdown = null;

    public bool HasDefaultName = false;
    public string NameChangeStringToIgnore = String.Empty;

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
                humanToggle.isOn = true;
                aiToggle.isOn = false;
            }
            else
            {
                humanToggle.isOn = false;
                aiToggle.isOn = true;
            }
        }
        get { return _profileIndex; }
    }

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
        Toolbox.Instance.playerProfiles[ProfileIndex].name = value;
        Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " + value);

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
            }
        }
        else
        {
            Toolbox.Instance.playerProfiles[ProfileIndex].type = PlayersProfile.Type.AI;
            if (nameField.text == string.Empty)
            {
                FindObjectOfType<GameSettingUIEvents>().SelectDefaultName(this);
            }
        }

        Debug.Log("Player [ " + ProfileIndex + " ] TYPE set to " + Toolbox.Instance.playerProfiles[this.ProfileIndex].type);
    }

    void OnAvatarChanged(int listValue)
    {
        Toolbox.Instance.playerProfiles[ProfileIndex].avatar = listValue;

        Debug.Log("Player [ " + ProfileIndex + " ] AVATAR set to " + Toolbox.Instance.playerProfiles[this.ProfileIndex].avatar);
    }
}
