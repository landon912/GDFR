using UnityEngine;
using System.Collections;

public class SwipeTest : MonoBehaviour {

	public UILabel label;
	public UICamera cam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Vector2 pos = UICamera.currentTouch.pos;
		//label.text = "test";
	}

	void OnDrag()
	{
		Vector2 pos = UICamera.currentTouch.totalDelta;
		label.text = pos.ToString();
	}
}
