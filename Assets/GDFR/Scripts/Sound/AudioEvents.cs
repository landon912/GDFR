using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioEvents : MonoBehaviour
{
    private const string FAIRY_LAUGH = "Fairy Laugh", GOBLIN_LAUGH = "Goblin Laugh";

	void OnEnable()
	{
	    EventReceiver.NewGameStartedEvent += OnNewGameStarted;
		EventReceiver.CardMovedEvent += OnCardMoved;
	    EventReceiver.CardPlayedEvent += OnCardPlayed;
		EventReceiver.StarPlayedEvent += OnStarPlayed;
		EventReceiver.SymbolMatchEvent += OnSymbolMatch;
	    EventReceiver.CardFlipEvent += OnCardFlip;
	    EventReceiver.CardsTakenEvent += OnCardsTaken;
        EventReceiver.PlayResultEvent += OnPlayResult;
	    EventReceiver.PlayerSelectEvent += OnPlayerSelect;
	    EventReceiver.ButtonPressedEvent += OnButtonPressed;
	    EventReceiver.DeclareWinnerEvent += OnDeclareWinner;
	}

	void OnDisable()
	{
	    EventReceiver.NewGameStartedEvent -= OnNewGameStarted;
		EventReceiver.CardMovedEvent -= OnCardMoved;
	    EventReceiver.CardPlayedEvent -= OnCardPlayed;
        EventReceiver.StarPlayedEvent -= OnStarPlayed;
		EventReceiver.SymbolMatchEvent -= OnSymbolMatch;
	    EventReceiver.CardFlipEvent -= OnCardFlip;
        EventReceiver.CardsTakenEvent -= OnCardsTaken;
		EventReceiver.PlayResultEvent -= OnPlayResult;
	    EventReceiver.PlayerSelectEvent -= OnPlayerSelect;
        EventReceiver.ButtonPressedEvent -= OnButtonPressed;
	    EventReceiver.DeclareWinnerEvent -= OnDeclareWinner;
    }

    void OnNewGameStarted()
    {
        StartCoroutine(PlayOneClipThenTheOther(FAIRY_LAUGH, GOBLIN_LAUGH, 0.5f));
    }

    void OnCardMoved(Card card)
    {
        AudioController.Play("Card Moved");
	}

    void OnCardPlayed(Card card)
    {
        //TODO: Replace with saying the name of the card
        switch (card.CurrentRace)
        {
            case Race.Fairy:
                AudioController.Play(FAIRY_LAUGH);
                break;
            case Race.Goblin:
                AudioController.Play(GOBLIN_LAUGH);
                break;
        }

        //AudioController.Play(card.NameSound);
    }

    void OnStarPlayed(Card card)
	{
	    AudioController.Play("Star Card");
	}

	void OnSymbolMatch(Card[] cards)
	{
	    AudioController.Play("Symbol Match");
	}

    void OnCardFlip(Card card, bool wasFromStar)
    {
        //TODO: Replace with saying the rhyme
        if(!wasFromStar) { AudioController.Play("Card Flip"); }
    }

    void OnCardsTaken(Card[] cards)
    {
        bool hasFairy = false, hasGoblin = false;

        foreach (Card card in cards)
        {
            if (hasFairy && hasGoblin){ break; }

            switch (card.CurrentRace)
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

	}

    void OnPlayerSelect(PlayersProfile newPlayer)
    {
        if (newPlayer.type == PlayersProfile.Type.Human)
        {
            AudioController.Play("Notification");
        }
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

    public void OnButtonPressed()
    {
        AudioController.Play(
            SceneManager.GetActiveScene().name == "MainGame" ? "Button Pressed" : "Menu Button Pressed");
    }

    public void OnGiveUpButtonPressed()
    {
        AudioController.Play("Children Sad");
    }

    public IEnumerator PlayOneClipThenTheOther(string firstClip, string secondClip, float delay)
    {
        yield return new WaitForSeconds(AudioController.Play(firstClip).clipLength + delay);
        AudioController.Play(secondClip);
    }
}