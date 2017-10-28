using UnityEngine;
using System.Collections;

public class AudioEvents : MonoBehaviour {

    new AudioSource audio;
    public AudioClip playedCardClip;
	public AudioClip playedStarClip;
	public AudioClip SymbolMatchClip;
	public AudioClip goodPlayClip;
	public AudioClip badPlayClip;

	void Start()
	{
		audio = GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		EventReceiver.cardPlayedEvent+=OnCardPlayed;
		EventReceiver.starPlayedEvent+=OnStarPlayed;
		EventReceiver.symbolMatchEvent+=OnSymbolMatch;
		EventReceiver.playResultEvent+=OnPlayResult;
	}

	void OnDisable()
	{
		EventReceiver.cardPlayedEvent-=OnCardPlayed;
		EventReceiver.starPlayedEvent-=OnStarPlayed;
		EventReceiver.symbolMatchEvent-=OnSymbolMatch;
		EventReceiver.playResultEvent-=OnPlayResult;
	}

	void OnCardPlayed(Card card)
	{
		audio.clip = playedCardClip;
		audio.Play();
	}

	void OnStarPlayed(Card card)
	{
		audio.clip = playedStarClip;
		audio.Play();
	}

	void OnSymbolMatch(Card[] cards)
	{
		audio.clip = SymbolMatchClip;
		audio.Play();
	}

	void OnPlayResult(int Quality)
	{
		Debug.Log("Result Hit");
		if(Quality>2)
		{
			audio.clip = goodPlayClip;
			audio.Play();
		}
		if(Quality<0)
		{
			audio.clip = badPlayClip;
			audio.Play();
		}
	}

}
