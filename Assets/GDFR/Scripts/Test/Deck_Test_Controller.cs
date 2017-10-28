using UnityEngine;
using System.Collections;

public class Deck_Test_Controller : MonoBehaviour {

	public static GDFR_Deck_Script masterDeck = null;
	public static GDFR_Deck_Script testDeck = null;
	public GDFR_Deck_Script _masterDeck = null;
	public GDFR_Deck_Script _testDeck = null;

	// Use this for initialization
	void Start () {
		masterDeck = _masterDeck;
		testDeck = _testDeck;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddRandomCard()
	{
		masterDeck.drawRandomCard(testDeck);
		testDeck.Refresh();
	}

	public void RemoveRandomCard()
	{
		testDeck.drawRandomCard(masterDeck);
		testDeck.Refresh();
	}
}
