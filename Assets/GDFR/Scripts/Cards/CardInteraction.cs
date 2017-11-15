using UnityEngine;

public class CardInteraction : MonoBehaviour {

	void OnClick()
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.UI_Card_Selected(card);
	}

	void OnDoubleClick()
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.OnCardDoubleClick(card);
	}

	void OnDrag(Vector2 delta)
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.OnCardDrag(card,delta);
	}

	void OnScroll(float delta)
	{
		Card card = GetComponent<Card>();
		UI_Event_Receiver.OnCardScroll(card,delta);
	}
}
