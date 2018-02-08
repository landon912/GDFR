using UnityEngine;

public class UI_Event_Receiver : MonoBehaviour
{
	public delegate void CardSelectedHandler(Card card);
	public static event CardSelectedHandler CardSelected;

	public delegate void CardDoubleClickedHandler(Card card);
	public static event CardDoubleClickedHandler CardDoubleClicked;

    public delegate void CardDraggedEventHandler(Vector2 pos);
    public static event CardDraggedEventHandler CardDragged;
    
    public delegate void UIScrollHandler(float delta);
    public static event UIScrollHandler UIScroll;

    public delegate void MuteButtonPressedHandler();
    public static event MuteButtonPressedHandler MuteButtonPressed;

    public delegate void HelpButtonPressedHandler();
    public static event HelpButtonPressedHandler HelpButtonPressed;

    public delegate void ExitButtonPressedHandler();
    public static event ExitButtonPressedHandler ExitButtonPressed;

    public delegate void ForfeitButtonPressedHandler();
    public static event ForfeitButtonPressedHandler ForfeitButtonPressed;

    public delegate void NewGameButtonPressedHandler();
    public static event NewGameButtonPressedHandler NewGameButtonPressed;

    public delegate void SetupButtonPressedHandler();
    public static event SetupButtonPressedHandler SetupButtonPressed;

    public static void TriggerCardSelectedEvent(Card card)
	{
		if(card != null && CardSelected != null)
			CardSelected(card);
	}

	public static void TriggerCardDoubleClickEvent(Card card)
	{
		if(card != null && CardDoubleClicked != null)
			CardDoubleClicked(card);
	}

	public static void TriggerCardDragEvent(Card card,Vector2 delta)
	{
	    if (CardDragged != null) CardDragged(delta);
	}

	public static void TriggerCardScrollEvent(Card card,float delta)
	{
	    if (UIScroll != null && card != null)
            UIScroll(delta);
	}

    public void TriggerMuteButtonPressedEvent()
    {
        if (MuteButtonPressed != null)
            MuteButtonPressed();
    }

    public void TriggerHelpButtonPressedEvent()
    {
        if (HelpButtonPressed != null)
            HelpButtonPressed();
    }

    public void TriggerExitButtonPressedEvent()
    {
        if (ExitButtonPressed != null)
            ExitButtonPressed();
    }

    public void TriggerForfeitButtonPressedEvent()
    {
        if (ForfeitButtonPressed != null)
            ForfeitButtonPressed();
    }

    public void TriggerNewGameButtonPressedEvent()
    {
        if (NewGameButtonPressed != null)
            NewGameButtonPressed();  
    }

    public void TriggerSetupButtonPressedEvent()
    {
        if (SetupButtonPressed != null)
            SetupButtonPressed();
    }
}