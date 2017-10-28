using UnityEngine;
using System.Collections;

public class PlayerProfile_UI : MonoBehaviour {

    public UnityEngine.UI.InputField nameField = null;
    public UnityEngine.UI.Toggle humanToggle = null;
    public UnityEngine.UI.Toggle aiToggle = null;
    public UnityEngine.UI.Dropdown avatarDropdown = null;

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

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Events
    void OnNameChanged(string value)
    {
        Toolbox.Instance.playerProfiles[this.ProfileIndex].name = value;
        Debug.Log("Player [ " + this.ProfileIndex + " ] NAME set to " + value);
    }

    void OnHumanToggle(bool value)
    {
        if (value)
        {
            Toolbox.Instance.playerProfiles[this.ProfileIndex].type = PlayersProfile.Type.Human;
        }
        else
        {
            Toolbox.Instance.playerProfiles[this.ProfileIndex].type = PlayersProfile.Type.AI;
        }

        Debug.Log("Player [ " + this.ProfileIndex + " ] TYPE set to " + Toolbox.Instance.playerProfiles[this.ProfileIndex].type);
    }

    void OnAvatarChanged(int listValue)
    {
        Toolbox.Instance.playerProfiles[this.ProfileIndex].avatar = listValue;

        Debug.Log("Player [ " + this.ProfileIndex + " ] AVATAR set to " + Toolbox.Instance.playerProfiles[this.ProfileIndex].avatar);
    }
}
