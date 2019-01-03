using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Avatar : MonoBehaviour 
{
	public Image avatarSprite = null;
	public Image avatarGlowSprite = null;
	public TextMeshProUGUI avatarLabel = null;

    public Sprite[] avatars;

    private Dictionary<string, Sprite> nameToAvatar = new Dictionary<string, Sprite>();

    private void Awake()
    {
        nameToAvatar.Add("Avatar_00", avatars[0]);
        nameToAvatar.Add("Avatar_01", avatars[1]);
        nameToAvatar.Add("Avatar_02", avatars[2]);
        nameToAvatar.Add("Avatar_03", avatars[3]);
        nameToAvatar.Add("Avatar_04", avatars[4]);
        nameToAvatar.Add("Avatar_05", avatars[5]);
        nameToAvatar.Add("Avatar_06", avatars[6]);
        nameToAvatar.Add("Avatar_07", avatars[7]);
        nameToAvatar.Add("Avatar_08", avatars[8]);
        nameToAvatar.Add("Avatar_09", avatars[9]);
        nameToAvatar.Add("Avatar_10", avatars[10]);
        nameToAvatar.Add("Avatar_AI", avatars[11]);
    }

    void Start()
    {
        Name = _name;
        spriteName = _spriteName;
    }

    public string _name = "Avatar";
	public string Name
	{
		set
		{
			_name = value;
			avatarLabel.text = _name;
		}
		get{return _name;}
	}

	public string _spriteName = "Avatar_01";
	public string spriteName
	{
		set
		{
			_spriteName = value;
            avatarSprite.sprite = nameToAvatar[_spriteName];
		}
		get{return _spriteName;}
	}
}