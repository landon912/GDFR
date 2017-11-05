using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameContoller : RxFx_FSM {
	
	public Object mainDeckXmlData;
	public GDFR_Deck_Script swapDeck;
	public GDFR_Deck_Script mainDeck;
	public GDFR_Deck_Script starDeck;
	public GDFR_Deck_Script fairyRingDeck;
	public GDFR_Deck_Script[] playerDeck;
    public Avatar[] avatars;
    public GDFR_Deck_Script playedCardDeck;
	public int currentPlayer = 0;
	public Card selectedCard = null;
	public GDFR_Card_Script playedCard = null;
    //delegate void selectEventHandler(Card card);
    //event selectEventHandler selectEvent; 
    //public float debugDelay = 0f;
    public float dealSpeed = 0.1f;
    public UI_Functions uiFunctionScript;
	public MonoBehaviour[] starEffectActivateList;
	//int _playResult = 0;
    int turnsCount = 0;
    List<int> playersPosition = new List<int>();
    public UILabel turnsCounter;

    protected float lastVolume = 0;

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

    public void NewGame()
    {
        // Clear all FSM
        this.StopAllCoroutines();
        RxFx_FSM.GlobalEventList.Clear();

        Destroy(AudioController.Instance.gameObject);
        SceneManager.LoadScene("NewGame");
    }

    public void ExitGame()
    {
        // Clear all FSM
        this.StopAllCoroutines();
        RxFx_FSM.GlobalEventList.Clear();

        Destroy(AudioController.Instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void MuteSound()
    {
        if (Math.Abs(lastVolume) < 0.005f)
        {
            lastVolume = AudioController.GetGlobalVolume();
            AudioController.SetGlobalVolume(0);
        }
        else
        {
            AudioController.SetGlobalVolume(lastVolume);
            lastVolume = 0;
        }
    }

    void OnEnable()
	{
	    UI_Event_Receiver.CardSelected += OnCardSelected;
    }

	void OnDestroy()
	{
		UI_Event_Receiver.CardSelected -= OnCardSelected;
    }

	void OnCardSelected(Card card)
	{
		callEvent("CardPicked",card);
	}

	IEnumerator State_LoadData(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_GameReset);	
		//load the deck data
		mainDeck.loadDeckData(mainDeckXmlData);

		callEvent("SettingUpRules");
		yield break;
	}

    IEnumerator State_SettingUpRules(params object[] data)
    {
        Debug.Log("Number of Players: " + Toolbox.Instance.gameSettings.numberOfPlayers + " - State: SettingUpRules");

        // Disable everything first
        foreach (GDFR_Deck_Script pDeck in playerDeck)
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
                playersPosition.Add(0);

                break;
            case 2:
                playersPosition.Add(0);
                playersPosition.Add(2);

                break;
            case 3:
                playersPosition.Add(0);
                playersPosition.Add(1);
                playersPosition.Add(2);

                break;
            case 4:
                playersPosition.Add(0);
                playersPosition.Add(1);
                playersPosition.Add(2);
                playersPosition.Add(3);

                break;
        }

        for(int playerIdx = 0; playerIdx < playersPosition.Count; playerIdx++)
        {
            int position = playersPosition[playerIdx];

            playerDeck[position].enabled = true;
            playerDeck[position].gameObject.SetActive(true);
            avatars[position].enabled = true;
            avatars[position].gameObject.SetActive(true);

            PlayersProfile playerProfile = Toolbox.Instance.playerProfiles[playerIdx];
            string name = playerProfile.name;
            if (name == "")
            {
                if (playerProfile.type == PlayersProfile.Type.Human)
                {
                    name = "Player " + (playerIdx + 1);
                }
            }
            avatars[position].Name = name;
            avatars[position].spriteName = "Avatar_" + playerProfile.avatar.ToString().PadLeft(2, '0');
            avatars[position].avatarGlowSprite.gameObject.SetActive(false);
        }

        callEvent("GameReset");
        yield break;
    }

    IEnumerator State_GameReset(params object[] data)
	{
        // Turns
        turnsCount = 0;
        turnsCounter.enabled = false;
        turnsCounter.gameObject.SetActive(false);

        //FSM_Event nextPhase = new FSM_Event("",State_DrawPhase1);	
        Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: GameReset");

		mainDeck.DeckUiEnabled(false);

		//return all cards to main deck
		for(int p=0;p<playerDeck.Length;p++)
		{
			playerDeck[p].ReturnAllCards(mainDeck);
		}
		fairyRingDeck.ReturnAllCards(mainDeck);
		swapDeck.ReturnAllCards(mainDeck);
		starDeck.ReturnAllCards(mainDeck);
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
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(uiFunctionScript.SendGameMessage(rulesMessage, 4f));


        //yield return new WaitForSeconds(2f);
	    EventReceiver.TriggerNewGameStartedEvent();
        yield return StartCoroutine(uiFunctionScript.SendGameMessage("New Game!",2f));

		callEvent("DrawPhase1");
	}

	//Remove Star Boarder Cards Give 1 random one to each player
	IEnumerator State_DrawPhase1(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_DrawPhase2);	
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: DrawPhase1");


        // Create the star deck
		GDFR_Card_Script[] cards = mainDeck.GetCardList() as GDFR_Card_Script[];
		foreach(GDFR_Card_Script c in cards)
		{
			c.currentRace = Race.Goblin;
			if(c.goblinStarBorder)
			{
				c.DrawCardInstant(starDeck);
			}
		}

        // Give 1 star card for each player
		foreach(GDFR_Deck_Script pDeck in playerDeck)
		{
            // Enabled player ?
            if (pDeck.enabled == true)
            {
                GDFR_Card_Script card = starDeck.drawRandomCard() as GDFR_Card_Script;
                yield return StartCoroutine(card.AnimateDrawCard(pDeck, dealSpeed));
            }
		}

        callEvent("DrawPhase2");
		yield break;
	}	

	//Draw Player Cards
	IEnumerator State_DrawPhase2(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_DrawPhase3);	
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: DrawPhase2");
		
		//draw 3 goblin cards to each player
		foreach(GDFR_Deck_Script pDeck in playerDeck)
		{
            // Enabled player ?
            if (pDeck.enabled == true)
            {
                int numberOfCards = 3; // classic mode

                // Solitaire modes?
                switch (Toolbox.Instance.gameSettings.rulesVariant)
                {
                    case GameSettings.RulesVariant.Solitaire:
                    case GameSettings.RulesVariant.Ultimate_Solitaire:
                        switch(Toolbox.Instance.gameSettings.difficultyLevel)
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

                for (int c = 0; c < numberOfCards; c++)
                {
                    GDFR_Card_Script card = mainDeck.drawRandomCard() as GDFR_Card_Script;
                    card.ChangeRace(Race.Goblin);
                    yield return StartCoroutine(card.AnimateDrawCard(pDeck, dealSpeed));
                }

			    //Card[] cards = pDeck.GetCardList();
			    //foreach(GDFR_Card_Script c in cards)
			    //	c.ChangeRace(Race.Goblin);
			    pDeck.Refresh();
            }
        }

		callEvent("DrawPhase3");
		yield break;
	}

	//Draw cards into the Fairy Row
	IEnumerator State_DrawPhase3(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_Initiative);	
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: DrawPhase3");

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
            foreach (GDFR_Card_Script c in starDeck.GetCardList())
            {
                c.DrawCardInstant(mainDeck);
            }
        }

        //draw 4 cards to the fairy ring and make them all fairies.
        for (int d=0; d < numberOfCards; d++)
		{
			//GDFR_Card_Script card = (GDFR_Card_Script)mainDeck.drawRandomCard(fairyRingDeck);
			GDFR_Card_Script card = (GDFR_Card_Script)mainDeck.drawRandomCard();
			card.currentRace = Race.Fairy;
			yield return StartCoroutine(card.AnimateDrawCard(fairyRingDeck,dealSpeed));
		}
		fairyRingDeck.Refresh();
		
		callEvent("Initiative");
		yield break;
	}
	
	IEnumerator State_Initiative(params object[] data)
	{
		//FSM_Event nextPhase = new FSM_Event("",State_PlayerSelect);	
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: Initiative");

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

        avatars[playersPosition[currentPlayer]].avatarGlowSprite.gameObject.SetActive(true);

        callEvent("PlayerSelect");
		yield break;
	}	
	
	IEnumerator State_PlayerSelect(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: PlayerSelect");

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

        //uiFunctionScript.StartCoroutine(uiFunctionScript.SendGameMessage("player " + (currentPlayer+1) + " Move!",3f));
        //yield return new WaitForSeconds(3f);

        if (turnsCounter.enabled)
        {
            turnsCount++;
            turnsCounter.text = "Turns: " + turnsCount;
        }

        // Is it AI ?
        if (Toolbox.Instance.playerProfiles[currentPlayer].type == PlayersProfile.Type.AI)
        {
            callEvent("AIMove");
            yield break;
        }
        else
        {
            callEvent("PlayerPickCard");
            yield break;
        }
	}	

	IEnumerator State_PlayerPickCard(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: State_PlayerPickCard");
        //FSM_Event cardPickedEvent = new FSM_Event("CardPicked",State_PlayerMove,true);

        playerDeck[playersPosition[currentPlayer]].DeckUiEnabled(true);
		playerDeck[playersPosition[currentPlayer]].VisuallyActive = true;
		playerDeck[playersPosition[currentPlayer]].zDepth = 600;
		yield return new WaitForSeconds(0.5f);

		while(true)
		{
			//Wait for player to select a card usinging CardPicked event externally.
			//Debug.Log ("Waiting");
			yield return null;
		}

		yield break;
	}

	IEnumerator State_PlayerMove(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: State_PlayerMove");

		//Get the select card from the event data;
		selectedCard = playedCard = (GDFR_Card_Script)data[0];
		playerDeck[playersPosition[currentPlayer]].DeckUiEnabled(false);
		//selectedCard.DrawCard(playedCardDeck);
		yield return StartCoroutine(playedCard.AnimateDrawCard(playedCardDeck,1.5f));
		callEvent("PlayResolve");

		yield break;
	}	

	IEnumerator State_AIMove(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: State_AIMove");

		playerDeck[playersPosition[currentPlayer]].VisuallyActive = true;
		playerDeck[playersPosition[currentPlayer]].zDepth = 600;
		yield return new WaitForSeconds(1.5f);

		//selectedCard = playedCard = playerDeck[playersPosition[currentPlayer]].drawRandomCard(playedCardDeck) as GDFR_Card_Script;
		//selectedCard = playedCard = playerDeck[playersPosition[currentPlayer]].drawRandomCard() as GDFR_Card_Script;
		selectedCard = playedCard = AI_PickBestCard(playerDeck[playersPosition[currentPlayer]],fairyRingDeck);


		yield return StartCoroutine(playedCard.AnimateDrawCard(playedCardDeck,1.5f));

		callEvent("PlayResolve");
		yield break;
	}	

	IEnumerator State_PlayResolve(params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: State_PlayResolve");

		//Let's see how good this move was
		int playQuality = GetPlayValue(playedCard,fairyRingDeck);

		Card[] fCard = fairyRingDeck.GetCardList();


		//Flip rhyming cards or star cards
		int Rhymecount = 0;
		bool cardFlipped = false;
		//if(playedCard.starsShowing)
			//yield return StartCoroutine(uiFunctionScript.SendGameMessage("player " + (currentPlayer+1) + "Played a stared card!",2f));
			//yield return new WaitForSeconds(2f);
		if(playedCard.starsShowing)
		{
			playedCard.PlayStarsEffect();
			EventReceiver.TriggerStarPlayedEvent(playedCard);
			//yield return new WaitForSeconds(0.5f);
			yield return new WaitForSeconds(1f);
		}
		foreach(GDFR_Card_Script c in fCard)
		{
			if(c!=playedCard)
			{
				if(c.currentRhyme==playedCard.currentRhyme || playedCard.starsShowing)
				{
					cardFlipped = true;
					Rhymecount++;
					//Debug.Log ("Rhyme Match Found");
					c.Flip();
				}
			}
			else
				Debug.Log ("Played Card Hit");
		}
		if(cardFlipped)
		{
		//	yield return StartCoroutine(uiFunctionScript.SendGameMessage("player " + (currentPlayer+1) + " Got " + Rhymecount + " Rhymes!",2f));
			yield return new WaitForSeconds(1f);
		}


		//colect matching symbols
		bool cardTaken = false;
		int cardCount = 0;
		List<GDFR_Card_Script> takenCards = new List<GDFR_Card_Script>();
		foreach(GDFR_Card_Script c in fCard)
		{
			if(c!=playedCard)
			{
				if(c.currentSymbol==playedCard.currentSymbol)
				{
					cardTaken = true;
					takenCards.Add(c);
					c.symbolMatchEffect();
					cardCount++;
				}
			}
		}

	    GDFR_Card_Script[] cList = new GDFR_Card_Script[takenCards.Count + 1];
		for(int tc=0;tc<takenCards.Count;tc++)
			cList[tc] = takenCards[tc];
		cList[takenCards.Count] = playedCard;
		EventReceiver.TriggerSymbolMatchEvent(cList);

		if(cardTaken)
		{
			playedCard.symbolMatchEffect();
            yield return new WaitForSeconds(2f);
		    EventReceiver.TriggerCardsTakenEvent(takenCards.ToArray());
        }

        foreach (GDFR_Card_Script c in takenCards)
		{
		    
            yield return StartCoroutine(c.AnimateDrawCard(playerDeck[playersPosition[currentPlayer]],0f));
		}
		fairyRingDeck.Refresh();
		fairyRingDeck.DeckUiEnabled(false);
		playerDeck[playersPosition[currentPlayer]].Refresh();


		//playedCard.DrawCard(fairyRingDeck);
		yield return StartCoroutine(playedCard.AnimateDrawCard(fairyRingDeck,1f));
		EventReceiver.TriggerPlayResultEvent(playQuality);
		yield return new WaitForSeconds(1f);
		fairyRingDeck.Refresh();

        switch (Toolbox.Instance.gameSettings.rulesVariant)
        {
            case GameSettings.RulesVariant.Classic:
                playerDeck[playersPosition[currentPlayer]].VisuallyActive = false;
                yield return new WaitForSeconds(0.5f);
                break;
        }
        //playerDeck[playersPosition[currentPlayer]].VisuallyActive = false;
        yield return new WaitForSeconds(0.5f);
        playerDeck[playersPosition[currentPlayer]].zDepth = 0;

        callEvent("CheckVictoryConditions");

		yield break;
	}	

	IEnumerator State_CheckVictoryConditions (params object[] data)
	{
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: State_CheckVictoryConditions");

		List<Card> cardList = new List<Card>();
        cardList.AddRange(playerDeck[playersPosition[currentPlayer]].GetCardList());

        // If ultimate, counts my deck and fairy ring deck
        if (Toolbox.Instance.gameSettings.rulesVariant == GameSettings.RulesVariant.Ultimate_Solitaire)
        {
            cardList.AddRange(fairyRingDeck.GetCardList());
        }

        int fairyCount = 0;
		int goblinCount = 0;
		foreach(GDFR_Card_Script c in cardList)
		{
			if(c.currentRace==Race.Fairy)
				fairyCount++;
			if(c.currentRace==Race.Goblin)
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
		//FSM_Event playerSelect = new FSM_Event("",State_PlayerSelect);
		
		Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: State_DeclareWinner " + currentPlayer);

	    EventReceiver.TriggerDeclareWinnerEvent(Toolbox.Instance.playerProfiles[currentPlayer]);
		yield return StartCoroutine(uiFunctionScript.SendGameMessage(Toolbox.Instance.playerProfiles[currentPlayer].name + " Wins!",2f));

		callEvent("GameReset");
	}

	IEnumerator State_ChangePlayer (params object[] data)
	{
        avatars[playersPosition[currentPlayer]].avatarGlowSprite.gameObject.SetActive(false);

		currentPlayer++;
		currentPlayer = currentPlayer % Toolbox.Instance.gameSettings.numberOfPlayers;

        avatars[playersPosition[currentPlayer]].avatarGlowSprite.gameObject.SetActive(true);

        Debug.Log("Player " + currentPlayer + "- Position: " + playersPosition[currentPlayer] + " - State: State_ChangePlayer " + currentPlayer);

		callEvent("PlayerSelect");
		yield break;
	}	

	public static GDFR_Card_Script AI_PickBestCard(GDFR_Deck_Script fromDeck,GDFR_Deck_Script toDeck)
	{
		//grab the first one.  Don't want to return null;
		GDFR_Card_Script currentBestCard = null;
		//start each fromDeck card
		int bestDiscardValue = -10;
		GDFR_Card_Script[] pCards = fromDeck.GetCardList() as GDFR_Card_Script[];
		//GDFR_Card_Script[] tCards = toDeck.GetCardList() as GDFR_Card_Script[];
		foreach(GDFR_Card_Script pCard in pCards)
		{
			int discardValue = GetPlayValue(pCard,toDeck);
			/*
			int discardValue = 0;
			if(pCard.currentRace==Race.Goblin)discardValue+=1;
			if(pCard.currentRace==Race.Fairy)discardValue-=1;
			//look through target deck and see how many fairies vs goblins we get

			foreach(GDFR_Card_Script tCard in tCards)
			{

				Symbol tSymbol = tCard.currentSymbol;
				Race tRace = tCard.currentRace;
				//is it a rhyme?
				if(tCard.currentRhyme==pCard.currentRhyme || pCard.starsShowing)
				{
					switch (tCard.currentSymbol)
					{
						case Symbol.Sun:
						tSymbol = Symbol.Moon;
						break;
						case Symbol.Moon:
						tSymbol = Symbol.Sun;
						break;
						case Symbol.Mushroom:
						tSymbol = Symbol.Frog;
						break;
						case Symbol.Frog:
						tSymbol = Symbol.Mushroom;
						break;
					}
					switch (tCard.currentRace)
					{
					case Race.Fairy:
						tRace = Race.Goblin;
						break;
					case Race.Goblin:
						tRace = Race.Fairy;
						break;
					}
				}

				//is it a match?
				if(tSymbol==pCard.currentSymbol)
				{
					if(tRace==Race.Goblin)
						discardValue-=1;
					else
						discardValue+=1;
				}
			}
			*/
			//Debug.Log("Card Value = " + discardValue);
			if(discardValue>bestDiscardValue)
			{
				currentBestCard = pCard;
				bestDiscardValue = discardValue;
			}
		}
		return currentBestCard;
	}

	public static int GetPlayValue(GDFR_Card_Script pCard, GDFR_Deck_Script toDeck)
	{
		GDFR_Card_Script[] tCards = toDeck.GetCardList() as GDFR_Card_Script[];
		int discardValue = 0;

		foreach(GDFR_Card_Script tCard in tCards)
		{
			Symbol tSymbol = tCard.currentSymbol;
			Race tRace = tCard.currentRace;

			// is it a rhyme or pCard = star?
            // invert symbol and race to make the calculation
			if(tCard.currentRhyme==pCard.currentRhyme || pCard.starsShowing)
			{
				switch (tCard.currentSymbol)
				{
				    case Symbol.Sun:
					    tSymbol = Symbol.Moon;
					    break;
				    case Symbol.Moon:
					    tSymbol = Symbol.Sun;
					    break;
				    case Symbol.Mushroom:
					    tSymbol = Symbol.Frog;
					    break;
				    case Symbol.Frog:
					    tSymbol = Symbol.Mushroom;
					    break;
				}
				switch (tCard.currentRace)
				{
				    case Race.Fairy:
					    tRace = Race.Goblin;
					    break;
				    case Race.Goblin:
					    tRace = Race.Fairy;
					    break;
				}
			}
			
			//is it a match?
			if(tSymbol==pCard.currentSymbol)
			{
				if(tRace==Race.Goblin)
					discardValue-=1;
				else
					discardValue+=1;
			}
		}

        if (pCard.currentRace == Race.Goblin)
        {
            // Best play is to discard goblins if it doesnt get more goblins
            if (discardValue == 0)
            {
                discardValue += 1;

                // In solitaire, best play is to discard goblins without get others
                if (Toolbox.Instance.gameSettings.numberOfPlayers == 1)
                {
                    discardValue += 3;
                }
            }
            discardValue += 1;
        }
        
        if (pCard.currentRace == Race.Fairy) discardValue -= 1;

        return discardValue;
	}
}