using UnityEngine;

public class UI_Event_Receiver : MonoBehaviour
{
	public delegate void CardSelectedHandler(Card card);
	public static event CardSelectedHandler CardSelected;

	public delegate void CardDoubleClickedHandler(Card card);
	public static event CardDoubleClickedHandler CardDoubleClicked;

	public delegate void BackgroundDoubleClickedHandler(Vector2 pos);
	public static event BackgroundDoubleClickedHandler BackgroundDoubleClicked;

	public delegate void BackgroundDraggedHandler(Vector2 pos);
	public static event BackgroundDraggedHandler BackgroundDragged;

	public delegate void BackgroundDroppedHandler();
	public static event BackgroundDroppedHandler BackgroundDropped;

	public delegate void UIScrollHandler(float delta);
	public static event UIScrollHandler UIScroll;

	public static void UI_Card_Selected(Card card)
	{
		if(card != null && CardSelected != null)
			CardSelected(card);
	}

	public static void OnCardClick(Card card)
	{
	}

	public static void OnCardDoubleClick(Card card)
	{
		if(card != null && CardDoubleClicked != null)
			CardDoubleClicked(card);
	}

	public static void OnCardDrag(Card card,Vector2 delta)
	{
		//BackgroundDragged(delta);
	}

	public static void OnCardScroll(Card card,float delta)
	{
	    if (UIScroll != null && card != null)
            UIScroll(delta);
	}

	void OnDoubleClick()
	{
	    if (BackgroundDoubleClicked != null) BackgroundDoubleClicked(UICamera.currentTouch.pos);
	}

	void OnDrag(Vector2 delta)
	{
	    if (BackgroundDragged != null) BackgroundDragged(delta);
	}

	void OnDrop()
	{
	    //Debug.Log("DROPPED");
	    if (BackgroundDropped != null) BackgroundDropped();
	}

	void OnScroll (float delta)
	{
	    if (UIScroll != null) UIScroll(delta);
	}
}
