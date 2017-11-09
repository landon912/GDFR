using UnityEngine;

public class EventReceiver : MonoBehaviour
{
    public delegate void NewGameStartedEventHandler();
    public static event NewGameStartedEventHandler NewGameStartedEvent;
    public static void TriggerNewGameStartedEvent()
    {
        if(NewGameStartedEvent != null) NewGameStartedEvent();
    }

	public delegate void CardMovedEventHandler(GDFR_Card_Script card);
	public static event CardMovedEventHandler CardMovedEvent; 
	public static void TriggerCardMovedEvent(GDFR_Card_Script card)
	{
	    if (CardMovedEvent != null) CardMovedEvent(card);
	}

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

    public delegate void CardFlipEventHandler(GDFR_Card_Script card);
    public static event CardFlipEventHandler CardFlipEvent;
    public static void TriggerCardFlipEvent(GDFR_Card_Script card)
    {
        if (CardFlipEvent != null) CardFlipEvent(card);
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

    public delegate void PlayerSelectEventHandler(PlayersProfile newPlayer);
    public static event PlayerSelectEventHandler PlayerSelectEvent;
    public static void TriggerPlayerSelectEvent(PlayersProfile newPlayer)
    {
        if (PlayerSelectEvent != null) PlayerSelectEvent(newPlayer);
    }

    public delegate void ButtonPressedEventHandler();
    public static event ButtonPressedEventHandler ButtonPressedEvent;
    public static void TriggerButtonPressedEvent()
    {
        if (ButtonPressedEvent != null) ButtonPressedEvent();
    }

    public delegate void DeclareWinnerEventHandler(PlayersProfile winner);
    public static event DeclareWinnerEventHandler DeclareWinnerEvent;
    public static void TriggerDeclareWinnerEvent(PlayersProfile winner)
    {
        if (DeclareWinnerEvent != null) DeclareWinnerEvent(winner);
    }
}