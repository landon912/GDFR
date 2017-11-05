using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioEvents : MonoBehaviour
{
    const string FAIRY_LAUGH = "Fairy Laugh", GOBLIN_LAUGH = "Goblin Laugh";

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
	    EventReceiver.NewGameStartedEvent += OnNewGameStarted;
		EventReceiver.CardPlayedEvent += OnCardPlayed;
		EventReceiver.StarPlayedEvent += OnStarPlayed;
		EventReceiver.SymbolMatchEvent += OnSymbolMatch;
	    EventReceiver.CardsTakenEvent += OnCardsTaken;
        EventReceiver.PlayResultEvent += OnPlayResult;
	    EventReceiver.PlayerSelectEvent += OnPlayerSelect;
	    EventReceiver.ButtonPressedEvent += OnButtonPressed;
	    EventReceiver.DeclareWinnerEvent += OnDeclareWinner;
	}

	void OnDisable()
	{
	    EventReceiver.NewGameStartedEvent -= OnNewGameStarted;
		EventReceiver.CardPlayedEvent -= OnCardPlayed;
		EventReceiver.StarPlayedEvent -= OnStarPlayed;
		EventReceiver.SymbolMatchEvent -= OnSymbolMatch;
	    EventReceiver.CardsTakenEvent -= OnCardsTaken;
		EventReceiver.PlayResultEvent -= OnPlayResult;
	    EventReceiver.PlayerSelectEvent += OnPlayerSelect;
        EventReceiver.ButtonPressedEvent -= OnButtonPressed;
	    EventReceiver.DeclareWinnerEvent -= OnDeclareWinner;
    }

    void OnNewGameStarted()
    {
        StartCoroutine(PlayOneClipThenTheOther(FAIRY_LAUGH, GOBLIN_LAUGH, 0.5f));
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

    void OnCardsTaken(GDFR_Card_Script[] cards)
    {
        bool hasFairy = false, hasGoblin = false;

        foreach (GDFR_Card_Script card in cards)
        {
            if (hasFairy && hasGoblin){ break; }

            switch (card.currentRace)
            {
                case Race.Fairy:
                    hasFairy = true;
                    break;
                case Race.Goblin:
                    hasGoblin = true;
                    break;
            }
        }

        if (hasFairy && hasGoblin)
        {
            StartCoroutine(PlayOneClipThenTheOther(FAIRY_LAUGH, GOBLIN_LAUGH, 0.5f));
        }
        else if (hasFairy)
        {
            AudioController.Play(FAIRY_LAUGH);
        }
        else if (hasGoblin)
        {
            AudioController.Play(GOBLIN_LAUGH);
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

    void OnPlayerSelect(PlayersProfile newPlayer)
    {
        if (newPlayer.type == PlayersProfile.Type.Human)
        {
            AudioController.Play("Notification");
        }
    }

    public void OnButtonPressed()
    {
        Debug.Log("Trying to play");
        AudioController.Play(
            SceneManager.GetActiveScene().name == "MainGame" ? "Button Pressed" : "Menu Button Pressed");
    }

    void OnDeclareWinner(PlayersProfile winner)
    {
        switch (winner.type)
        {
            case PlayersProfile.Type.Human:
                AudioController.Play("Children Cheer");
                break;
            case PlayersProfile.Type.AI:
                AudioController.Play("Children Sad");
                break;
        }
    }

    public IEnumerator PlayOneClipThenTheOther(string firstClip, string secondClip, float delay)
    {
        yield return new WaitForSeconds(AudioController.Play(firstClip).clipLength + delay);
        AudioController.Play(secondClip);
    }
}