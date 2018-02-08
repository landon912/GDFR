using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour 
{
	public UISprite avatarSprite = null;
	public UISprite avatarGlowSprite = null;
	public UILabel avatarLabel = null;
	
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
			avatarSprite.spriteName = _spriteName;
		}
		get{return _spriteName;}
	}

	void Start () 
	{
		Name = _name;
		spriteName = _spriteName;
	}
}