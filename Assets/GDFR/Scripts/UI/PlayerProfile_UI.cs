using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile_UI : MonoBehaviour {

    public InputField nameField = null;
    public Text nameStatic = null;
    public Toggle humanToggle = null;
    public Toggle aiToggle = null;
    public Dropdown avatarDropdown = null;

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
        Toolbox.Instance.playerProfiles[ProfileIndex].name = value;
        Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " + Toolbox.Instance.playerProfiles[ProfileIndex].name);
    }

    public void OnHumanToggle(bool isHuman)
    {
        if (isHuman)
        {
            //enable text field, disable static name
            nameField.gameObject.SetActive(true);
            nameStatic.transform.parent.gameObject.SetActive(false);

            Toolbox.Instance.playerProfiles[ProfileIndex].type = PlayersProfile.Type.Human;
            nameField.text = "Player " + (ProfileIndex + 1);
            avatarDropdown.value = 0;
            avatarDropdown.interactable = true;

            Toolbox.Instance.playerProfiles[ProfileIndex].name = "Player " + (ProfileIndex + 1);
            Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " +
                      Toolbox.Instance.playerProfiles[ProfileIndex].name);
        }
        else
        {
            //enable static name, disable dyanmic field
            nameStatic.transform.parent.gameObject.SetActive(true);
            nameField.gameObject.SetActive(false);

            Toolbox.Instance.playerProfiles[ProfileIndex].type = PlayersProfile.Type.AI;
            FindObjectOfType<GameSettingUIEvents>().SelectDefaultProfile(this);
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

        Debug.Log("Player [ " + ProfileIndex + " ] AVATAR set to " + Toolbox.Instance.playerProfiles[ProfileIndex].avatar);

    }
}
