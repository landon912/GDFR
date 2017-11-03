using UnityEngine;

public class EventReceiver : MonoBehaviour {

	public delegate void CardPlayedEventHandler(Card card);
	public static event CardPlayedEventHandler CardPlayedEvent; 
	public static void TriggerCardPlayedEvent(Card card)
	{
	    if (CardPlayedEvent != null) CardPlayedEvent(card);
	}

	public delegate void StarPlayedEventHandler(Card card);
	public static event StarPlayedEventHandler StarPlayedEvent; 
	public static void TriggerStarPlayedEvent(Card card)
	{
	    if (StarPlayedEvent != null) StarPlayedEvent(card);
	}

	public delegate void SymbolMatchEventHandler(Card[] cards);
	public static event SymbolMatchEventHandler SymbolMatchEvent; 
	public static void TriggerSymbolMatchEvent(Card[] cards)
	{
	    if (SymbolMatchEvent != null) SymbolMatchEvent(cards);
	}

    public delegate void CardTakenEventHandler(Card card);
    public static event CardTakenEventHandler CardTakenEvent;
    public static void TriggerCardTakenEvent(Card card)
    {
        if (CardTakenEvent != null) CardTakenEvent(card);
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