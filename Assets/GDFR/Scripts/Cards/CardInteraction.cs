using UnityEngine;
using System.Collections;

public class CardInteraction : MonoBehaviour {

	void OnClick()
	{
		GDFR_Card_Script card = GetComponent<GDFR_Card_Script>();
		UI_Event_Receiver.UI_Card_Selected(card);
	}

	void OnDoubleClick()
	{
		GDFR_Card_Script card = GetComponent<GDFR_Card_Script>();
		UI_Event_Receiver.OnCardDoubleClick(card);
	}

	void OnDrag(Vector2 delta)
	{
		GDFR_Card_Script card = GetComponent<GDFR_Card_Script>();
		UI_Event_Receiver.OnCardDrag(card,delta);
	}

	void OnScroll(float delta)
	{
		GDFR_Card_Script card = GetComponent<GDFR_Card_Script>();
		UI_Event_Receiver.OnCardScroll(card,delta);
	}
}
