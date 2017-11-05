using UnityEngine;

public class UI_Event_Receiver : MonoBehaviour {

	public delegate void CardSelectedHandler(Card card);
	public static event CardSelectedHandler CardSelected;

	public delegate void CardDoubleClickedHandler(Card card);
	public static event CardDoubleClickedHandler CardDoubleClicked;

	public delegate void backgroundDoubleClickedHandler(Vector2 pos);
	public static event backgroundDoubleClickedHandler BackgroundDoubleClicked;

	public delegate void backgroundDraggedHandler(Vector2 pos);
	public static event backgroundDraggedHandler BackgroundDragged;

	public delegate void backgroundDroppedHandler();
	public static event backgroundDroppedHandler BackgroundDropped;

	public delegate void UIScrollHandler(float delta);
	public static event UIScrollHandler UIScroll;

	public static void UI_Card_Selected(Card card)
	{
		if(card!=null && CardSelected!=null)
			CardSelected(card);
	}

	public static void OnCardClick(Card card)
	{
	}

	public static void OnCardDoubleClick(Card card)
	{
		if(card!=null && CardDoubleClicked!=null)
			CardDoubleClicked(card);
	}

	public static void OnCardDrag(Card card,Vector2 delta)
	{
		//BackgroundDragged(delta);
	}

	public static void OnCardScroll(Card card,float delta)
	{
		UIScroll(delta);
	}

	void OnDoubleClick()
	{
		//Debug.Log ("Something happened");
		BackgroundDoubleClicked(UICamera.currentTouch.pos);
	}

	void OnDrag(Vector2 delta)
	{
		BackgroundDragged(delta);
	}

	void OnDrop()
	{
		//Debug.Log("DROPPED");
		BackgroundDropped();
	}

	void OnScroll (float delta)
	{
		UIScroll(delta);
	}
}
