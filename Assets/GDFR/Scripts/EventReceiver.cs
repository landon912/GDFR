using UnityEngine;
using System.Collections;

public class EventReceiver : MonoBehaviour {

	public delegate void cardPlayedEventHandler(Card card);
	public static event cardPlayedEventHandler cardPlayedEvent; 
	public static void TriggerCardPlayedEvent(Card card){cardPlayedEvent(card);}

	public delegate void starPlayedEventHandler(Card card);
	public static event starPlayedEventHandler starPlayedEvent; 
	public static void TriggerStarPlayedEvent(Card card){starPlayedEvent(card);}

	public delegate void symbolMatchEventHandler(Card[] cards);
	public static event symbolMatchEventHandler symbolMatchEvent; 
	public static void TriggerSymbolMatchEvent(Card[] cards){symbolMatchEvent(cards);}

	public delegate void playResultEventHandler(int resultValue);
	public static event playResultEventHandler playResultEvent; 
	public static void TriggerPlayResultEvent(int resultValue){playResultEvent(resultValue);}
}
