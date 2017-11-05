using System.Collections;
using System.Collections.Generic;
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
	    EventReceiver.CardsTakenEvent += OnCardsTaken;
        EventReceiver.PlayResultEvent += OnPlayResult;
	    EventReceiver.ButtonPressedEvent += OnButtonPressed;
	}

	void OnDisable()
	{
		EventReceiver.CardPlayedEvent -= OnCardPlayed;
		EventReceiver.StarPlayedEvent -= OnStarPlayed;
		EventReceiver.SymbolMatchEvent -= OnSymbolMatch;
	    EventReceiver.CardsTakenEvent -= OnCardsTaken;
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

    void OnCardsTaken(GDFR_Card_Script[] cards)
    {
        bool hasFairy = false, hasGoblin = false;
        const string FAIRY_LAUGH = "Fairy Laugh", GOBLIN_LAUGH = "Goblin Laugh";

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

    IEnumerator PlayOneClipThenTheOther(string firstClip, string secondClip, float delay)
    {
        AudioObject playingAudio = AudioController.Play(firstClip);
        yield return new WaitForSeconds(playingAudio.clipLength + delay);
        AudioController.Play(secondClip);
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