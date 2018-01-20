﻿using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AvatarOption
{
    public int id;
    public Sprite graphic;
}

public class PlayerProfile_UI : MonoBehaviour {

    public InputField nameField = null;
    public Text nameStatic = null;
    public Toggle humanToggle = null;
    public Toggle aiToggle = null;
    public AvatarSelector avatarSelector;
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

    [HideInInspector]
    public bool PlayToggleSound = true;

    // Registering Events
    void OnEnable()
    {
        nameField.onEndEdit.AddListener(OnNameChanged);
        humanToggle.onValueChanged.AddListener(OnHumanToggle);
        //avatarDropdown.onValueChanged.AddListener(ChangeAvatar);
    }

    // De-registering events
    void OnDisable()
    {
        nameField.onEndEdit.RemoveAllListeners();
        humanToggle.onValueChanged.RemoveAllListeners();
        //avatarDropdown.onValueChanged.RemoveAllListeners();
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

            ChangeAvatar(0);
            avatarSprite.GetComponent<Button>().interactable = true;

            Toolbox.Instance.playerProfiles[ProfileIndex].name = "Player " + (ProfileIndex + 1);
            Debug.Log("Player [ " + ProfileIndex + " ] NAME set to " +
                      Toolbox.Instance.playerProfiles[ProfileIndex].name);
        }
        else
        {
            //enable static name, disable dyanmic field
            nameStatic.transform.parent.gameObject.SetActive(true);
            nameField.gameObject.SetActive(false);

            avatarSprite.GetComponent<Button>().interactable = false;

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

    public void ChangeAvatar(int id)
    {
        Toolbox.Instance.playerProfiles[ProfileIndex].avatar = id;
        avatarSprite.sprite = AvatarOptions[id].graphic;

        Debug.Log("Player [ " + ProfileIndex + " ] AVATAR set to " + Toolbox.Instance.playerProfiles[ProfileIndex].avatar);
    }

    public void OpenAvatarSelector()
    {
        avatarSelector.Show();
    }
}
