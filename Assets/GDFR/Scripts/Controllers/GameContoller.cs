using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameContoller : RxFx_FSM
{
    public GameObject GiveUpButtonGameObject;
	public Object mainDeckXmlData;
	public Deck swapDeck;
	public Deck mainDeck;
	public Deck starDeck;
	public Deck fairyRingDeck;
	public Deck[] playerDecks;
    public Avatar[] avatars;
    public Deck playedCardDeck;
	public int currentPlayer;
	public Card selectedCard;
	public Card playedCard;
    public float dealSpeed = 0.1f;
    public UI_Functions uiFunctionScript;
	public MonoBehaviour[] starEffectActivateList;
    public UILabel turnsCounter;
    public ButtonSpriteSwaper muteButtonSpriteSwaper;
    public UICamera mainUICamera;

    private int mTurnsCount;
    private AIModule mAIModule;

    private readonly List<int> mPlayersPosition = new List<int>();
    private readonly List<Deck> mManuallyControlledDecks = new List<Deck>();

    protected float lastVolume;

	void Awake()
	{
        new FSM_Event("StartEvent",State_LoadData);
        new FSM_Event("SettingUpRules", State_SettingUpRules);
        new FSM_Event("GameReset",State_GameReset);
		new FSM_Event("DrawPhase1",State_DrawPhase1);	
		new FSM_Event("DrawPhase2",State_DrawPhase2);
		new FSM_Event("DrawPhase3",State_DrawPhase3);
		new FSM_Event("Initiative",State_Initiative);
		new FSM_Event("PlayerSelect",State_PlayerSelect);	
		new FSM_Event("PlayerPickCard",State_PlayerPickCard);
		new FSM_Event("CardPicked",State_PlayerMove);
		new FSM_Event("AIMove",State_AIMove);
		new FSM_Event("CheckVictoryConditions",State_CheckVictoryConditions);			
		new FSM_Event("ChangePlayer",State_ChangePlayer);
		new FSM_Event("DeclareWinner",State_DeclareWinner);
		new FSM_Event("PlayResolve",State_PlayResolve);

        callEvent("StartEvent");
	}

    void Start()
    {
        //The main menu's track's probability is 0 at the beginning to keep it from being selected; since at this point, another track has been selected we can renable it's probability
	    AudioController.Instance._GetAudioItem("Game Music").subItems[0].Probability = 100;
    }

    private void NewGame()
    {
        // Clear all FSM
        StopAllCoroutines();
        GlobalEventList.Clear();

        Destroy(AudioController.Instance.gameObject);
        SceneManager.LoadScene("NewGame");
    }

    private void ExitGame()
    {
        // Clear all FSM
        StopAllCoroutines();
        GlobalEventList.Clear();

        Destroy(AudioController.Instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    private void MuteSound()
    {
        if (Math.Abs(lastVolume) < 0.005f)
        {
            muteButtonSpriteSwaper.Swap();
            lastVolume = AudioController.GetGlobalVolume();
            AudioController.SetGlobalVolume(0);
        }
        else
        {
            muteButtonSpriteSwaper.Swap();
            AudioController.SetGlobalVolume(lastVolume);
            lastVolume = 0;
        }
    }

    private void LoadHelpMenu()
    {
        SceneManager.LoadSceneAsync("Help_Additive", LoadSceneMode.Additive);
    }

    private void ForfeitGame()
    {
        callEvent("GameReset");
        GiveUpButtonGameObject.SetActive(false);
    }

    void OnEnable()
	{
	    UI_Event_Receiver.CardSelected += OnCardSelected;
	    UI_Event_Receiver.MuteButtonPressed += MuteSound;
	    UI_Event_Receiver.HelpButtonPressed += LoadHelpMenu;
	    UI_Event_Receiver.ExitButtonPressed += NewGame;
	    UI_Event_Receiver.ForfeitButtonPressed += ForfeitGame;
	}

	void OnDestroy()
	{
		UI_Event_Receiver.CardSelected -= OnCardSelected;
	    UI_Event_Receiver.MuteButtonPressed -= MuteSound;
	    UI_Event_Receiver.HelpButtonPressed -= LoadHelpMenu;
	    UI_Event_Receiver.ExitButtonPressed -= NewGame;
	    UI_Event_Receiver.ForfeitButtonPressed -= ForfeitGame;
    }

    void OnCardSelected(Card card)
	{
		callEvent("CardPicked",card);
	}

	IEnumerator State_LoadData(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_GameReset);	
		//load the deck data
		mainDeck.LoadDeckData(mainDeckXmlData);

		callEvent("SettingUpRules");
		yield break;
	}

    IEnumerator State_SettingUpRules(params object[] data)
    {
        Debug.Log("Number of Players: " + Toolbox.Instance.gameSettings.numberOfPlayers + " - State: SettingUpRules");

        // Disable everything first
        foreach (Deck pDeck in playerDecks)
        {
            pDeck.gameObject.SetActive(false);
            pDeck.enabled = false;
        }
        foreach (Avatar avatar in avatars)
        {
            avatar.gameObject.SetActive(false);
            avatar.enabled = false;
        }

        switch (Toolbox.Instance.gameSettings.numberOfPlayers)
        {
            case 1:
                mPlayersPosition.Add(0);
                break;
            case 2:
                mPlayersPosition.Add(0);
                mPlayersPosition.Add(2);
                break;
            case 3:
                mPlayersPosition.Add(0);
                mPlayersPosition.Add(1);
                mPlayersPosition.Add(2);
                break;
            case 4:
                mPlayersPosition.Add(0);
                mPlayersPosition.Add(1);
                mPlayersPosition.Add(2);
                mPlayersPosition.Add(3);
                break;
        }

        for(int playerIdx = 0; playerIdx < mPlayersPosition.Count; playerIdx++)
        {
            int position = mPlayersPosition[playerIdx];

            playerDecks[position].enabled = true;
            playerDecks[position].gameObject.SetActive(true);
            avatars[position].enabled = true;
            avatars[position].gameObject.SetActive(true);

            PlayersProfile playerProfile = Toolbox.Instance.playerProfiles[playerIdx];

            if (playerProfile.type == PlayersProfile.Type.Human)
            {
                mManuallyControlledDecks.Add(playerDecks[position]);
            }

            avatars[position].Name = playerProfile.name;
            avatars[position].spriteName = "Avatar_" + playerProfile.avatar.ToString().PadLeft(2, '0');
            avatars[position].avatarGlowSprite.gameObject.SetActive(false);
        }

        callEvent("GameReset");
        yield break;
    }

    IEnumerator State_GameReset(params object[] data)
	{
        // Turns
        mTurnsCount = 0;
        turnsCounter.enabled = false;
        turnsCounter.gameObject.SetActive(false);

        //FSM_Event nextPhase = new FSM_Event("",State_DrawPhase1);	
        Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: GameReset");

		mainDeck.DeckUiEnabled(false);

	    foreach (int index in mPlayersPosition)
	    {
	        avatars[index].avatarGlowSprite.gameObject.SetActive(false);
	    }

        //return all cards to main deck
        for (int p=0;p<playerDecks.Length;p++)
		{
			playerDecks[p].ReturnAllCards(mainDeck);
		}
		fairyRingDeck.ReturnAllCards(mainDeck);
		swapDeck.ReturnAllCards(mainDeck);
		starDeck.ReturnAllCards(mainDeck);
	    playedCardDeck.ReturnAllCards(mainDeck);
		mainDeck.CollapseDeck();

        string rulesMessage = "";
        switch(Toolbox.Instance.gameSettings.rulesVariant)
        {
            case GameSettings.RulesVariant.Solitaire:
                rulesMessage = "Get rid of all your Goblins, in as few turns as possible!"; break;
            case GameSettings.RulesVariant.Ultimate_Solitaire:
                rulesMessage = "Get rid of all Goblins in your hand, and in the fairy ring!"; break;
            case GameSettings.RulesVariant.Classic:
                switch(Toolbox.Instance.gameSettings.numberOfPlayers)
                {
                    case 2:
                        rulesMessage = "Be the first player to get rid of all your Goblins, or collect 8 Fairies!"; break;
                    case 3:
                    case 4:
                        rulesMessage = "Be the first player to get rid of all your Goblins, or collect 7 Fairies!"; break;
                    default:
                        Debug.LogError("You Should Not Be Here - Invalid Number of Players in this Rule Variant"); break;
                }
                break;
            case GameSettings.RulesVariant.Goblins_Rule:
                switch (Toolbox.Instance.gameSettings.numberOfPlayers)
                {
                    case 2:
                        rulesMessage = "Be the first player to get rid of all your Fairies, or collect 8 Goblins!"; break;
                    case 3:
                    case 4:
                        rulesMessage = "Be the first player to get rid of all your Fairies, or collect 7 Goblins!"; break;
                    default:
                        Debug.LogError("You Should Not Be Here - Invalid Number of Players in this Rule Variant"); break;
                }
                break;
            default:
                Debug.LogError("You Should Not Be Here - Invalid Rule Variant"); break;
        }

	    switch (Toolbox.Instance.gameSettings.difficultyLevel)
	    {
	        case GameSettings.Difficulty.Easy:
	            mAIModule = new EasyAIModule();
                break;
	        case GameSettings.Difficulty.Medium:
                mAIModule = new MediumAIModule();
	            break;
	        case GameSettings.Difficulty.Hard:
                mAIModule = new HardAIModule();
	            break;
	        case GameSettings.Difficulty.Very_Hard:
                mAIModule = new VeryHardAIModule();
	            break;
	        default:
	            throw new ArgumentOutOfRangeException();
	    }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(uiFunctionScript.SendGameMessage(rulesMessage, 4f));

        //yield return new WaitForSeconds(2f);
	    EventReceiver.TriggerNewGameStartedEvent();
        yield return StartCoroutine(uiFunctionScript.SendGameMessage("New Game!",2f));

	    switch(Toolbox.Instance.gameSettings.rulesVariant)
	    {
            case GameSettings.RulesVariant.Solitaire:
            case GameSettings.RulesVariant.Ultimate_Solitaire:
                GiveUpButtonGameObject.SetActive(true);
            break;
	    }

	    callEvent("DrawPhase1");
	}

	//Remove Star Boarder Cards Give 1 random one to each player
	IEnumerator State_DrawPhase1(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_DrawPhase2);	
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: DrawPhase1");

        // Create the star deck
	    Card[] cards = mainDeck.GetCardList();
		foreach(Card c in cards)
		{
		    if (Toolbox.Instance.gameSettings.rulesVariant == GameSettings.RulesVariant.Goblins_Rule)
		    {
			    c.CurrentRace = Race.Fairy;
		        if (c.fairyStarBorder)
		        {
		            c.DrawCardInstant(starDeck);
		        }
            }
		    else
		    {
			    c.CurrentRace = Race.Goblin;
		        if (c.goblinStarBorder)
		        {
		            c.DrawCardInstant(starDeck);
		        }
            }
		}

        // Give 1 star card for each player
		foreach(Deck pDeck in playerDecks)
		{
            // Enabled player ?
            if (pDeck.enabled)
            {
                Card card = starDeck.DrawRandomCard();
                yield return StartCoroutine(card.AnimateDrawCard(pDeck, dealSpeed));
            }
		}

        callEvent("DrawPhase2");
	}	

	//Draw Player Cards
	IEnumerator State_DrawPhase2(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_DrawPhase3);	
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: DrawPhase2");

	    GameSettings.RulesVariant rulesVariant = Toolbox.Instance.gameSettings.rulesVariant;

        int numberOfCards; // classic mode

	    // Solitaire modes?
	    switch (rulesVariant)
	    {
	        case GameSettings.RulesVariant.Solitaire:
	        case GameSettings.RulesVariant.Ultimate_Solitaire:
	            switch (Toolbox.Instance.gameSettings.difficultyLevel)
	            {
	                default:
	                case GameSettings.Difficulty.Easy:
	                    numberOfCards = 4;
	                    break;
	                case GameSettings.Difficulty.Medium:
	                    numberOfCards = 5;
	                    break;
	                case GameSettings.Difficulty.Hard:
	                    numberOfCards = 6;
	                    break;
	                case GameSettings.Difficulty.Very_Hard:
	                    numberOfCards = 7;
	                    break;
	            }
	            break;
	        case GameSettings.RulesVariant.Classic:
	        default:
	            switch (Toolbox.Instance.gameSettings.numberOfPlayers)
	            {
	                case 2:
	                    numberOfCards = 5;
	                    break;
	                case 3:
	                    numberOfCards = 4;
	                    break;
	                default:
	                    numberOfCards = 3;
	                    break;
	            }
	            break;
	    }

        //For balance reasons, select a random card from the symbol combo OPPOSITE of the dealt star card's symbol combo
        // Ex. If you get a Sun/Moon, your next card should be a random Frog/Mushroom card
        //Then, draw random goblins
        foreach (Deck pDeck in playerDecks)
		{
            // Enabled player ?
            if (pDeck.enabled)
            {
                Card secondCard = mainDeck.DrawRandomCardOfSymbolGroup(pDeck.GetCardList()[0].CurrentSymbolGroup == SymbolGroup.FrogMushroom ? SymbolGroup.SunMoon : SymbolGroup.FrogMushroom);
                secondCard.ChangeRace(rulesVariant == GameSettings.RulesVariant.Goblins_Rule ? Race.Fairy : Race.Goblin);
                yield return StartCoroutine(secondCard.AnimateDrawCard(pDeck, dealSpeed));

                //deal the rest of the cards
                for (int c = 1; c < numberOfCards; c++)
                {
                    Card card = mainDeck.DrawRandomCard();
                    card.ChangeRace(rulesVariant == GameSettings.RulesVariant.Goblins_Rule ? Race.Fairy : Race.Goblin);
                    yield return StartCoroutine(card.AnimateDrawCard(pDeck, dealSpeed));
                }

			    pDeck.Refresh();
            }
        }

		callEvent("DrawPhase3");
	}

	//Draw cards into the Fairy Row
	IEnumerator State_DrawPhase3(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_Initiative);	
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: DrawPhase3");

        int numberOfCards = 4; // classic mode
        // Solitaire modes?
        switch (Toolbox.Instance.gameSettings.rulesVariant)
        {
            case GameSettings.RulesVariant.Solitaire:
            case GameSettings.RulesVariant.Ultimate_Solitaire:
                switch (Toolbox.Instance.gameSettings.difficultyLevel)
                {
                    default:
                    case GameSettings.Difficulty.Easy:
                        numberOfCards = 5;
                        break;
                    case GameSettings.Difficulty.Medium:
                        numberOfCards = 6;
                        break;
                    case GameSettings.Difficulty.Hard:
                        numberOfCards = 7;
                        break;
                    case GameSettings.Difficulty.Very_Hard:
                        numberOfCards = 8;
                        break;
                }
                break;
            case GameSettings.RulesVariant.Classic:
            default:
                switch (Toolbox.Instance.gameSettings.numberOfPlayers)
                {
                    case 2:
                        numberOfCards = 6;
                        break;
                    case 3:
                        numberOfCards = 5;
                        break;
                    default:
                        numberOfCards = 4;
                        break;
                }
                break;
        }

        // If the number of players is above 2 (equal 3), return remaining star cards to the mainDeck.
        if (Toolbox.Instance.gameSettings.numberOfPlayers > 2)
        {
            foreach (Card c in starDeck.GetCardList())
            {
                c.DrawCardInstant(mainDeck);
            }
        }

        //draw 4 cards to the fairy ring and make them all fairies.
        for (int d=0; d < numberOfCards; d++)
		{
			Card card = mainDeck.DrawRandomCard();
			card.CurrentRace = Toolbox.Instance.gameSettings.rulesVariant == GameSettings.RulesVariant.Goblins_Rule ? Race.Goblin : Race.Fairy;
            yield return StartCoroutine(card.AnimateDrawCard(fairyRingDeck,dealSpeed));
		}
		fairyRingDeck.Refresh();
		
		callEvent("Initiative");
	}
	
	IEnumerator State_Initiative(params object[] data)
	{	
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: Initiative");

        // pick a random player IF Difficulty isn't easy
        // if so, get a human player to start
        if (Toolbox.Instance.gameSettings.difficultyLevel == GameSettings.Difficulty.Easy)
        {
            bool foundPlayer = false;
            while (!foundPlayer)
            {
                bool noHuman = true;
                for (int idx = 0; idx < Toolbox.Instance.gameSettings.numberOfPlayers; idx++)
                {
                    if ((Toolbox.Instance.playerProfiles[idx].type == PlayersProfile.Type.Human) && (Random.Range(0, 2) == 0))
                    {
                        foundPlayer = true;
                        currentPlayer = idx;
                        break;
                    }

                    if (Toolbox.Instance.playerProfiles[idx].type == PlayersProfile.Type.Human)
                    {
                        noHuman = false;
                    }
                }
                if (noHuman) break;
            }
        }
        else
        {
            currentPlayer = Random.Range(0, Toolbox.Instance.gameSettings.numberOfPlayers);
        }

        avatars[mPlayersPosition[currentPlayer]].avatarGlowSprite.gameObject.SetActive(true);

        callEvent("PlayerSelect");
		yield break;
	}	
	
	IEnumerator State_PlayerSelect(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: PlayerSelect");

	    EventReceiver.TriggerPlayerSelectEvent(Toolbox.Instance.playerProfiles[currentPlayer]);

        switch (Toolbox.Instance.gameSettings.rulesVariant)
        {
            case GameSettings.RulesVariant.Classic:
                turnsCounter.enabled = false;
                turnsCounter.gameObject.SetActive(false);
                break;
            case GameSettings.RulesVariant.Solitaire:
            case GameSettings.RulesVariant.Ultimate_Solitaire:
                turnsCounter.enabled = true;
                turnsCounter.gameObject.SetActive(true);
                break;
        }

        if (turnsCounter.enabled)
        {
            mTurnsCount++;
            turnsCounter.text = "Turns: " + mTurnsCount;
        }

        // Is it AI ?
        if (Toolbox.Instance.playerProfiles[currentPlayer].type == PlayersProfile.Type.AI)
        {
            callEvent("AIMove");
        }
        else
        {
            callEvent("PlayerPickCard");
            yield break;
        }
	}	

	IEnumerator State_PlayerPickCard(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: State_PlayerPickCard");

        playerDecks[mPlayersPosition[currentPlayer]].DeckUiEnabled(true);
		playerDecks[mPlayersPosition[currentPlayer]].VisuallyActive = true;
		playerDecks[mPlayersPosition[currentPlayer]].zDepth = 600;
		yield return new WaitForSeconds(0.5f);

		while(true)
		{
			//Wait for player to select a card usinging CardPicked event externally.
			yield return null;
		}
	}

	IEnumerator State_PlayerMove(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: State_PlayerMove");

		//Get the select card from the event data;
		selectedCard = playedCard = (Card)data[0];
		playerDecks[mPlayersPosition[currentPlayer]].DeckUiEnabled(false);

        EventReceiver.TriggerCardPlayedEvent(playedCard);

        yield return StartCoroutine(playedCard.AnimateDrawCard(playedCardDeck,1.5f));

        callEvent("PlayResolve");
	}	

	IEnumerator State_AIMove(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: State_AIMove");

		playerDecks[mPlayersPosition[currentPlayer]].VisuallyActive = true;
		playerDecks[mPlayersPosition[currentPlayer]].zDepth = 600;
		yield return new WaitForSeconds(1.5f);

		//selectedCard = playedCard = playerDecks[playersPosition[currentPlayer]].DrawRandomCard(playedCardDeck) as Card;
		//selectedCard = playedCard = playerDecks[playersPosition[currentPlayer]].DrawRandomCard() as Card;
		selectedCard = playedCard = AI_PickBestCard(playerDecks[mPlayersPosition[currentPlayer]],fairyRingDeck);

	    EventReceiver.TriggerCardPlayedEvent(playedCard);

        yield return StartCoroutine(playedCard.AnimateDrawCard(playedCardDeck,1.5f));

        callEvent("PlayResolve");
	}	

	IEnumerator State_PlayResolve(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: State_PlayResolve");

		//Let's see how good this move was
		//int playQuality = GetPlayValue(playedCard,fairyRingDeck);

		Card[] fCard = fairyRingDeck.GetCardList();

		//Flip rhyming cards or star cards
		int Rhymecount = 0;
		bool cardFlipped = false;

        if (playedCard.StarsShowing)
		{
			playedCard.PlayStarsEffect();
			EventReceiver.TriggerStarPlayedEvent(playedCard);
			yield return new WaitForSeconds(1f);
		}

        //go from right to left
        for(int i = fCard.Length-1; i>=0; i--)
		{
		    Card c = fCard[i];
			if(c!=playedCard)
			{
				if(c.CurrentRhyme==playedCard.CurrentRhyme || playedCard.StarsShowing)
				{
					cardFlipped = true;
					Rhymecount++;
				    if (playedCard.StarsShowing)
				    {
                        //faster flip
				        StartCoroutine(c.Flip(playedCard.StarsShowing));
				        yield return new WaitForSeconds(c.cardFlipTweenerA.duration / 3f);
                    }
				    else
				    {
                        //slow flip
				        yield return StartCoroutine(c.Flip(playedCard.StarsShowing));
				    }
				}
			}
			else
				Debug.Log ("Played Card Hit");
		}
		if(cardFlipped)
		{
			yield return new WaitForSeconds(1f);
		}

		//colect matching symbols
		bool cardTaken = false;
		int cardCount = 0;
		List<Card> takenCards = new List<Card>();
		foreach(Card c in fCard)
		{
			if(c!=playedCard)
			{
				if(c.CurrentSymbol==playedCard.CurrentSymbol)
				{
					cardTaken = true;
					takenCards.Add(c);
					c.SymbolMatchEffect();
					cardCount++;
				}
			}
		}

	    Card[] cList = new Card[takenCards.Count + 1];
		for(int tc=0;tc<takenCards.Count;tc++)
			cList[tc] = takenCards[tc];
		cList[takenCards.Count] = playedCard;
		EventReceiver.TriggerSymbolMatchEvent(cList);

		if(cardTaken)
		{
			playedCard.SymbolMatchEffect();
            yield return new WaitForSeconds(2f);
		    EventReceiver.TriggerCardsTakenEvent(takenCards.ToArray());
        }

        foreach (Card c in takenCards)
		{
            yield return StartCoroutine(c.AnimateDrawCard(playerDecks[mPlayersPosition[currentPlayer]],0f));
		}
		fairyRingDeck.Refresh();
		fairyRingDeck.DeckUiEnabled(false);
		playerDecks[mPlayersPosition[currentPlayer]].Refresh();

		yield return StartCoroutine(playedCard.AnimateDrawCard(fairyRingDeck,1f));
		//EventReceiver.TriggerPlayResultEvent(playQuality);
		yield return new WaitForSeconds(1f);
		fairyRingDeck.Refresh();

        switch (Toolbox.Instance.gameSettings.rulesVariant)
        {
            case GameSettings.RulesVariant.Classic:
            case GameSettings.RulesVariant.Goblins_Rule:
                playerDecks[mPlayersPosition[currentPlayer]].VisuallyActive = false;
                yield return new WaitForSeconds(0.5f);
                break;
        }

        yield return new WaitForSeconds(0.5f);
        playerDecks[mPlayersPosition[currentPlayer]].zDepth = 0;

        callEvent("CheckVictoryConditions");
	}	

	IEnumerator State_CheckVictoryConditions (params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: State_CheckVictoryConditions");

		List<Card> cardList = new List<Card>();
        cardList.AddRange(playerDecks[mPlayersPosition[currentPlayer]].GetCardList());

        // If ultimate, counts my deck and fairy ring deck
        if (Toolbox.Instance.gameSettings.rulesVariant == GameSettings.RulesVariant.Ultimate_Solitaire)
        {
            cardList.AddRange(fairyRingDeck.GetCardList());
        }

        int fairyCount = 0;
		int goblinCount = 0;
		foreach(Card c in cardList)
		{
			if(c.CurrentRace==Race.Fairy)
				fairyCount++;
			if(c.CurrentRace==Race.Goblin)
				goblinCount++;
		}

        switch (Toolbox.Instance.gameSettings.rulesVariant)
        {
            
            case GameSettings.RulesVariant.Classic:
                switch (Toolbox.Instance.gameSettings.numberOfPlayers)
                {
                    case 2:
                        if (fairyCount >= 8 || goblinCount == 0)
                        {
                            callEvent("DeclareWinner");
                            yield break;
                        }
                        break;
                    case 3:
                    case 4:
                        if (fairyCount >= 7 || goblinCount == 0)
                        {
                            callEvent("DeclareWinner");
                            yield break;
                        }
                        break;
                    default:
                        Debug.LogError("You Should Not Be Here - There is a RuleVariant and Player Number Mismatch or Player Count is out of range");
                        break;
                }
                break;
            case GameSettings.RulesVariant.Goblins_Rule:
                switch (Toolbox.Instance.gameSettings.numberOfPlayers)
                {
                    case 2:
                        if (goblinCount >= 8 || fairyCount == 0)
                        {
                            callEvent("DeclareWinner");
                            yield break;
                        }
                        break;
                    case 3:
                    case 4:
                        if (goblinCount >= 7 || fairyCount == 0)
                        {
                            callEvent("DeclareWinner");
                            yield break;
                        }
                        break;
                    default:
                        Debug.LogError("You Should Not Be Here - There is a RuleVariant and Player Number Mismatch or Player Count is out of range");
                        break;
                }
                break;
            case GameSettings.RulesVariant.Ultimate_Solitaire:
            case GameSettings.RulesVariant.Solitaire:
                if (goblinCount == 0)
                {
                    callEvent("DeclareWinner");
                    yield break;
                }
                break;
            default:
                Debug.LogError("You Should Not Be Here - Rules Variant is Unknown");
                break;
        }

		callEvent("ChangePlayer");
	}	

	IEnumerator State_DeclareWinner (params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: State_DeclareWinner " + currentPlayer);

	    EventReceiver.TriggerDeclareWinnerEvent(Toolbox.Instance.playerProfiles[currentPlayer]);
		yield return StartCoroutine(uiFunctionScript.SendGameMessage(Toolbox.Instance.playerProfiles[currentPlayer].name + " Wins!",2f));

		callEvent("GameReset");
	}

	IEnumerator State_ChangePlayer (params object[] data)
	{
        avatars[mPlayersPosition[currentPlayer]].avatarGlowSprite.gameObject.SetActive(false);

		currentPlayer++;
		currentPlayer = currentPlayer % Toolbox.Instance.gameSettings.numberOfPlayers;

        avatars[mPlayersPosition[currentPlayer]].avatarGlowSprite.gameObject.SetActive(true);

        Debug.Log("Player " + currentPlayer + "- Position: " + mPlayersPosition[currentPlayer] + " - State: State_ChangePlayer " + currentPlayer);

		callEvent("PlayerSelect");
		yield break;
	}	

	public Card AI_PickBestCard(Deck fromDeck,Deck toDeck)
	{
		//grab the first one.  Don't want to return null;
		Card currentBestCard = null;
		//start each fromDeck card
		int bestDiscardValue = -10;
	    Card[] pCards = fromDeck.GetCardList();
		foreach(Card pCard in pCards)
		{
			int discardValue = mAIModule.GetPlayValue(pCard, toDeck, Toolbox.Instance.gameSettings.rulesVariant == GameSettings.RulesVariant.Goblins_Rule ? -1 : 1, mManuallyControlledDecks);
			
			if(discardValue>bestDiscardValue)
			{
				currentBestCard = pCard;
				bestDiscardValue = discardValue;
			}
		}
		return currentBestCard;
	}
}