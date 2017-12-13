using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {

	public Transform CardAnchor = null;
	//public Camera camera;
	public Vector2 zoomMinMax = new Vector2(0.5f,1f);

	void Update()
	{
		//ZoomPosition = testPosition;
	}

	void OnEnable()
	{
		//UI_Event_Receiver.BackgroundDoubleClicked+=OnBackgroundDoubleClicked;
		//UI_Event_Receiver.BackgroundDragged+=OnBackgroundDragged;
		//UI_Event_Receiver.BackgroundDropped+=OnBackgroundDropped;
		UI_Event_Receiver.UIScroll+=OnUiScroll;
	}

	void OnDissable()
	{
		//UI_Event_Receiver.BackgroundDoubleClicked-=OnBackgroundDoubleClicked;
		//UI_Event_Receiver.BackgroundDragged-=OnBackgroundDragged;
		//UI_Event_Receiver.BackgroundDropped-=OnBackgroundDropped;
		UI_Event_Receiver.UIScroll-=OnUiScroll;
	}

	void OnBackgroundDoubleClicked(Vector2 pos)
	{
		Debug.Log(pos.ToString());
		pos = new Vector3(pos.x - Screen.width/2,pos.y - Screen.height/2 ,0f);
		//Debug.Log("Background was doubleclicked " + pos.ToString());
	}

	void OnBackgroundDragged(Vector2 pos)
	{
		//Debug.Log(pos);
		CardAnchor.localPosition+=new Vector3(pos.x,pos.y,0f) * GetComponent<Camera>().orthographicSize;
	}

	void OnBackgroundDropped()
	{
		//dragging = false;
		//Debug.Log("Drag Stopped");
	}

	void OnUiScroll(float delta)
	{
		//Debug.Log("Scroll");
		float newSize = GetComponent<Camera>().orthographicSize + delta;
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(newSize,zoomMinMax.x,zoomMinMax.y);
	}

	void UpdatePosition(Vector2 pos)
	{
		//Transform child = transform.GetChild(0);
		//Vector3 temp = new Vector3(pos.x,pos.y,0f);
		//child.position = -temp;
		//transform.position = temp;
	}
}
