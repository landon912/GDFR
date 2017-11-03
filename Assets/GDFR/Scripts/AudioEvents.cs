using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioEvents : MonoBehaviour {

    public AudioSource audio;
    public AudioClip playedCardClip;
	public AudioClip goodPlayClip;
	public AudioClip badPlayClip;

	void Start()
	{
		audio = GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		EventReceiver.CardPlayedEvent += OnCardPlayed;
		EventReceiver.StarPlayedEvent += OnStarPlayed;
		EventReceiver.SymbolMatchEvent += OnSymbolMatch;
	    EventReceiver.CardTakenEvent += OnCardTaken;
        EventReceiver.PlayResultEvent += OnPlayResult;
	    EventReceiver.ButtonPressedEvent += OnButtonPressed;
	}

	void OnDisable()
	{
		EventReceiver.CardPlayedEvent -= OnCardPlayed;
		EventReceiver.StarPlayedEvent -= OnStarPlayed;
		EventReceiver.SymbolMatchEvent -= OnSymbolMatch;
	    EventReceiver.CardPlayedEvent -= OnCardTaken;
		EventReceiver.PlayResultEvent -= OnPlayResult;
	    EventReceiver.ButtonPressedEvent -= OnButtonPressed;
    }

    void OnCardPlayed(Card card)
	{
		audio.clip = playedCardClip;
		audio.Play();
	}

	void OnStarPlayed(Card card)
	{
	    AudioController.Play("Star Card");
	}

	void OnSymbolMatch(Card[] cards)
	{
	    AudioController.Play("Symbol Match");
	}

    void OnCardTaken(Card card)
    {
        GDFR_Card_Script c = (GDFR_Card_Script) card;
        switch (c.currentRace)
        {
            case Race.Fairy:
                AudioController.Play("Fairy Laugh");
                break;
            case Race.Goblin:
                //AudioController.Play("Goblin Laugh");
                break;
        }
    }

	void OnPlayResult(int quality)
	{
		Debug.Log("Result Hit");
		if(quality > 2)
		{
			audio.clip = goodPlayClip;
			audio.Play();
		}
		if(quality < 0)
		{
			audio.clip = badPlayClip;
			audio.Play();
		}
	}

    public void OnButtonPressed()
    {
        Debug.Log("Trying to play");
        AudioController.Play(
            SceneManager.GetActiveScene().name == "MainGame" ? "Button Pressed" : "Menu Button Pressed");
    }
}
