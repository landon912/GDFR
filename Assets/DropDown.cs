using UnityEngine;
using System.Collections;



public class DropDown : MonoBehaviour {

	public delegate void DropDownChangedEvent(string selection);
	public event DropDownChangedEvent OnDropDownChanged;
	UIPopupList popup;

	void Awake()
	{
		popup = GetComponent<UIPopupList>();
	}

	public void OnValueChange()
	{
		if(OnDropDownChanged!=null)
			OnDropDownChanged(popup.value);
	}
	
}
