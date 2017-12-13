using UnityEngine;

public class CardInteraction : MonoBehaviour {

	void OnClick()
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.TriggerCardSelectedEvent(card);
	}

	void OnDoubleClick()
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.TriggerCardDoubleClickEvent(card);
	}

	void OnDrag(Vector2 delta)
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.TriggerCardDragEvent(card,delta);
	}

	void OnScroll(float delta)
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.TriggerCardScrollEvent(card,delta);
	}
}
