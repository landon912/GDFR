using UnityEngine;

public class EventReceiver : MonoBehaviour {

	public delegate void CardPlayedEventHandler(GDFR_Card_Script card);
	public static event CardPlayedEventHandler CardPlayedEvent; 
	public static void TriggerCardPlayedEvent(GDFR_Card_Script card)
	{
	    if (CardPlayedEvent != null) CardPlayedEvent(card);
	}

	public delegate void StarPlayedEventHandler(GDFR_Card_Script card);
	public static event StarPlayedEventHandler StarPlayedEvent; 
	public static void TriggerStarPlayedEvent(GDFR_Card_Script card)
	{
	    if (StarPlayedEvent != null) StarPlayedEvent(card);
	}

	public delegate void SymbolMatchEventHandler(GDFR_Card_Script[] cards);
	public static event SymbolMatchEventHandler SymbolMatchEvent; 
	public static void TriggerSymbolMatchEvent(GDFR_Card_Script[] cards)
	{
	    if (SymbolMatchEvent != null) SymbolMatchEvent(cards);
	}

    public delegate void CardsTakenEventHandler(GDFR_Card_Script[] cards);
    public static event CardsTakenEventHandler CardsTakenEvent;
    public static void TriggerCardsTakenEvent(GDFR_Card_Script[] cards)
    {
        if (CardsTakenEvent != null) CardsTakenEvent(cards);
    }

    public delegate void PlayResultEventHandler(int resultValue);
	public static event PlayResultEventHandler PlayResultEvent; 
	public static void TriggerPlayResultEvent(int resultValue)
	{
	    if (PlayResultEvent != null) PlayResultEvent(resultValue);
	}

    public delegate void ButtonPressedEventHandler();
    public static event ButtonPressedEventHandler ButtonPressedEvent;
    public static void TriggerButtonPressedEvent()
    {
        if (ButtonPressedEvent != null) ButtonPressedEvent();
    }
}