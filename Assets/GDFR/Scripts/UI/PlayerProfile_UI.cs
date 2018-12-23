using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AvatarOption
{
    public int id;
    public Sprite graphic;
}

public class PlayerProfile_UI : MonoBehaviour, IRepresentClient
{
    private int mClientId = -1;

    public InputField nameField = null;
    public Text nameStatic = null;
    public Toggle humanToggle = null;
    public Toggle aiToggle = null;
    public bool isNetworkPlayer = false;
    public Image avatarSprite = null;

    public AvatarOption[] AvatarOptions;

    [HideInInspector]
    public AIData defaultProfileAssigned = null;


    public int CurrentAvatarID
    {
        get { return Toolbox.Instance.playerProfiles[ProfileIndex].avatar; }
    }

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
                ChangeAvatar(Toolbox.Instance.playerProfiles[value].avatar);
            }

            //local player
            if (Toolbox.Instance.playerProfiles[value].type == PlayersProfile.Type.Human && !IsRepresentingNetworkClient())
            {
                PlayToggleSound = false;

                humanToggle.isOn = true;
                aiToggle.isOn = false;

                PlayToggleSound = true;
            }
            //ai
            else if(!IsRepresentingNetworkClient())
            {
                PlayToggleSound = false;

                humanToggle.isOn = false;
                aiToggle.isOn = true;

                PlayToggleSound = true;
            }
            //network player
            else
            {
                PlayToggleSound = false;

                humanToggle.isOn = false;
                aiToggle.isOn = false;

                PlayToggleSound = true;
            }
        }
        get { return _profileIndex; }
    }

    [HideInInspector]
    public bool PlayToggleSound = true;

    // Registering Events
    void OnEnable()
    {
        nameField.onEndEdit.AddListener(OnNameChanged);
        humanToggle.onValueChanged.AddListener(OnHumanToggle);
    }

    // De-registering events
    void OnDisable()
    {
        nameField.onEndEdit.RemoveAllListeners();
        humanToggle.onValueChanged.RemoveAllListeners();
    }

    // Events
    public void OnNameChanged(string value)
    {
        Toolbox.Instance.playerProfiles[ProfileIndex].name = value;

        GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.SetupNameChanged, new PlayerNameMessage(ProfileIndex, value));

        Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " + Toolbox.Instance.playerProfiles[ProfileIndex].name);
    }

    public void OnHumanToggle(bool isHuman)
    {
        if (isHuman)
        {
            //enable text field, disable static name
            SetStaticLabelStatus(false);
            nameField.text = "Player " + (ProfileIndex + 1);

            Toolbox.Instance.playerProfiles[ProfileIndex].type = PlayersProfile.Type.Human;

            ChangeAvatar(0);
            avatarSprite.GetComponent<Button>().interactable = true;

            Toolbox.Instance.playerProfiles[ProfileIndex].name = "Player " + (ProfileIndex + 1);
            Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " +
                      Toolbox.Instance.playerProfiles[ProfileIndex].name);
        }
        else
        {
            //enable static name, disable dyanmic field
            SetStaticLabelStatus(true);

            nameField.text = "AI Player " + (ProfileIndex + 1);
            nameStatic.text = "AI Player " + (ProfileIndex + 1);

            avatarSprite.GetComponent<Button>().interactable = false;

            Toolbox.Instance.playerProfiles[ProfileIndex].type = PlayersProfile.Type.AI;
            FindObjectOfType<GameSettingUIEvents>().SelectDefaultProfile(this);
        }

        //skip us manually changing the first profile to a player automatically
        if (PlayToggleSound)
        {
            EventReceiver.TriggerButtonPressedEvent();
        }

        GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.SetupHumanToggleChanged, new PlayerToggleMessage(ProfileIndex, isHuman));

        Debug.Log("Player [ " + ProfileIndex + " ] TYPE set to " + Toolbox.Instance.playerProfiles[ProfileIndex].type);
    }

    private void SetStaticLabelStatus(bool isStatic, bool disableToggle = false)
    {
        nameField.gameObject.SetActive(!isStatic);
        nameStatic.transform.parent.gameObject.SetActive(isStatic);
        humanToggle.interactable = !disableToggle;
    }

    public void ChangeAvatar(int id)
    {
        OnAvatarChanged(id);

        if(IsRepresentingLocalClient())
        {
            GDFRNetworkManager.Instance.TriggerEventIfClient(MsgIndexes.SetupAvatarChanged, new PlayerAvatarMessage(ProfileIndex, id));
        }
        else if(IsAI())
        {
            GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.SetupAvatarChanged, new PlayerAvatarMessage(ProfileIndex, id));
        }

        Debug.Log("Player [ " + ProfileIndex + " ] AVATAR set to " + Toolbox.Instance.playerProfiles[ProfileIndex].avatar);
    }

    public void OnAvatarChanged(int id)
    {
        Toolbox.Instance.playerProfiles[ProfileIndex].avatar = id;
        avatarSprite.sprite = AvatarOptions[id].graphic;
    }

    public void OpenAvatarSelector()
    {
        FindObjectOfType<AvatarSelector>().Show(this);
    }

    public void SetAsAIOnly()
    {
        humanToggle.isOn = false;
        humanToggle.gameObject.SetActive(false);
        aiToggle.isOn = true;
        aiToggle.interactable = false;
    }

    /// IRepresentClient implementations
    public bool IsRepresentingClientId(int clientId)
    {
        return mClientId == clientId;
    }

    private bool IsRepresentingLocalClient()
    {
        return GDFRNetworkManager.Instance.LocalConnectionId == mClientId;
    }

    public void SetAsRepresentingClientId(int clientId)
    {
        mClientId = clientId;

        SetStaticLabelStatus(true, true);

        aiToggle.isOn = false;

        nameStatic.text = GDFRNetworkManager.Instance.networkProfiles[clientId].networkName;
        OnNameChanged(GDFRNetworkManager.Instance.networkProfiles[clientId].networkName);
    }

    public bool IsAI()
    {
        return aiToggle.isOn;
    }

    public bool IsRepresentingNetworkClient()
    {
        return mClientId != -1;
    }
    /// End IRepresentClient implementations
}