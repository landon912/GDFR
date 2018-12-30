using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class GDFRNetworkGameManager : MonoBehaviour
{
    private static GDFRNetworkGameManager mInstance;
    public static GDFRNetworkGameManager Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = FindObjectOfType<GDFRNetworkGameManager>();
            return mInstance;
        }
    }

    private GameContoller mController;
    private GroupedDrawMessage phase1DrawData;
    private GroupedDrawMessage phase2DrawData;
    private GroupedDrawMessage phase3DrawData;
    private IntMessage initiativeData;


    private void Awake()
    {
        mController = FindObjectOfType<GameContoller>();
    }

    private void OnEnable()
    {
        SetupBaseMessageHandlers();
        SetupServerMessageHandlers();
    }

    public void SetupBaseMessageHandlers()
    {
        //GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.DrawCard, NetOnDrawCard);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.GroupedDrawMessage, NetOnGroupDrawMessage);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.InitiativeSelected, NetOnRecieveInitiative);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.CardPlayed, NetOnCardPlayed);
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.StartNewGame, NetOnStartNewGame);
    }

    public void SetupServerMessageHandlers()
    {
        //NetworkServer.RegisterHandler(MsgType.Ready, NetOnClientReady);
    }

    private void OnDisable()
    {
        //GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.DrawCard);
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.GroupedDrawMessage);
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.InitiativeSelected);
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.CardPlayed);
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.StartNewGame);

        //NetworkServer.UnregisterHandler(MsgType.Ready);
    }

    public bool IsCurrentPlayerTheLocalClient()
    {
        return Toolbox.Instance.playerProfiles[mController.currentPlayer].networkProfile.networkId == GDFRNetworkManager.Instance.LocalConnectionId;
    }

    private IEnumerator HandleGroupDrawData(GroupedDrawMessage data)
    {
        //handle data once we get it from server
        Deck fromDeck = mController.GetDeckFromId(data.fromDeck);

        for (int i = 0; i < data.cardIds.Length; i++)
        {
            Card c = fromDeck.GetExactCard(data.cardIds[i]);
            Deck toDeck = mController.GetDeckFromId(data.toDeckIds[i]);
            yield return StartCoroutine(c.AnimateDrawCard(toDeck, mController.dealSpeed));
        }
    }

    public IEnumerator State_Network_DrawPhase1()
    {
        Debug.Log("NETWORK: " + " Player " + mController.currentPlayer + "- Position: " + mController.PlayersPosition[mController.currentPlayer] + " - State: DrawPhase1");

        //give 1 star card to each deck
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost())
        {
            List<int> cards = new List<int>();
            List<int> toDecks = new List<int>();

            // Give 1 star card for each player
            foreach (Deck pDeck in mController.playerDecks)
            {
                // Enabled player ?
                if (pDeck.enabled)
                {
                    Card card = mController.starDeck.DrawRandomCard();

                    //build phase 1 order list
                    cards.Add(card.Id);
                    toDecks.Add(pDeck.Id);
                }
            }

            //send phase1 data
            GroupedDrawMessage outMess = new GroupedDrawMessage(GroupDrawPhase.Phase1, mController.starDeck.Id, cards, toDecks);

            GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.GroupedDrawMessage, outMess);
        }

        //handle data
        while (phase1DrawData == null)
        {
            Debug.Log("Waiting for data");
            yield return null;
        }

        yield return StartCoroutine(HandleGroupDrawData(phase1DrawData));
    }

    public IEnumerator State_Network_DrawPhase2()
    {
        GameSettings.RulesVariantType rulesVariantType = Toolbox.Instance.gameSettings.RulesVariant;

        int numberOfCards = mController.DeterminePlayerCardCount();

        int fromDeckId = -1;
        List<int> cardsId = new List<int>();
        List<int> toDecksId = new List<int>();

        if (GDFRNetworkManager.Instance.IsLocalClientTheHost())
        {
            //For balance reasons, select a random card from the symbol combo OPPOSITE of the dealt star card's symbol combo
            // Ex. If you get a Sun/Moon, your next card should be a random Frog/Mushroom card
            //Then, draw random goblins
            foreach (Deck pDeck in mController.playerDecks)
            {
                // Enabled player ?
                if (pDeck.enabled)
                {
                    Card secondCard = mController.mainDeck.DrawRandomCardOfSymbolGroup(pDeck.GetCardList()[0].CurrentSymbolGroup == SymbolGroup.FrogMushroom ? SymbolGroup.SunMoon : SymbolGroup.FrogMushroom);
                    secondCard.ChangeRace(rulesVariantType == GameSettings.RulesVariantType.GoblinsRule ? Race.Fairy : Race.Goblin);

                    fromDeckId = mController.mainDeck.Id;
                    cardsId.Add(secondCard.Id);
                    toDecksId.Add(pDeck.Id);

                    //deal the rest of the cards
                    for (int c = 1; c < numberOfCards; c++)
                    {
                        Card card = mController.mainDeck.DrawRandomCard();
                        card.ChangeRace(rulesVariantType == GameSettings.RulesVariantType.GoblinsRule ? Race.Fairy : Race.Goblin);

                        cardsId.Add(card.Id);
                        toDecksId.Add(pDeck.Id);
                    }

                    pDeck.Refresh();

                    GroupedDrawMessage outMess = new GroupedDrawMessage(GroupDrawPhase.Phase2, fromDeckId, cardsId, toDecksId);

                    GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.GroupedDrawMessage, outMess);
                }
            }
        }

        //wait until we get data
        while (phase2DrawData == null)
        {
            Debug.Log("Waiting for data in phase2");
            yield return null;
        }

        yield return StartCoroutine(HandleGroupDrawData(phase2DrawData));
    }

    public IEnumerator State_Network_DrawPhase3()
    {
        List<int> cards = new List<int>();
        List<int> toDecks = new List<int>();

        if (GDFRNetworkManager.Instance.IsLocalClientTheHost())
        {
            int numberOfCards = mController.DetermineFairyRowCardCount();

            //draw cards to the fairy ring and make them all fairies.
            for (int d = 0; d < numberOfCards; d++)
            {
                Card card = mController.mainDeck.DrawRandomCard();

                cards.Add(card.Id);
                toDecks.Add(mController.fairyRingDeck.Id);
            }

            GroupedDrawMessage outMess = new GroupedDrawMessage(GroupDrawPhase.Phase3, mController.mainDeck.Id, cards, toDecks);

            GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.GroupedDrawMessage, outMess);
        }

        while (phase3DrawData == null)
        {
            Debug.Log("waiting for data in phase3");
            yield return null;
        }

        Deck fromDeck = mController.GetDeckFromId(phase3DrawData.fromDeck);
        for (int i = 0; i < phase3DrawData.cardIds.Length; i++)
        {
            Card c = fromDeck.GetExactCard(phase3DrawData.cardIds[i]);
            c.CurrentRace = Toolbox.Instance.gameSettings.RulesVariant == GameSettings.RulesVariantType.GoblinsRule ? Race.Goblin : Race.Fairy;
            Deck toDeck = mController.GetDeckFromId(phase3DrawData.toDeckIds[i]);
            yield return StartCoroutine(c.AnimateDrawCard(toDeck, mController.dealSpeed));
        }

        //mController.fairyRingDeck.Refresh();
    }

    public IEnumerator State_Network_Initiative()
    {
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost())
        {
            // pick a random player IF Difficulty isn't easy
            // if so, get a human player to start
            if (Toolbox.Instance.gameSettings.DifficultyLevel == GameSettings.Difficulty.Easy)
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

                            GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.InitiativeSelected, new IntMessage(idx));
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
                GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.InitiativeSelected, new IntMessage(Random.Range(0, Toolbox.Instance.gameSettings.numberOfPlayers)));
            }
        }

        while (initiativeData == null)
        {
            Debug.Log("waiting for data in initiative");
            yield return null;
        }

        mController.avatars[mController.PlayersPosition[mController.currentPlayer]].avatarGlowSprite.gameObject.SetActive(true);
    }

    //public IEnumerator State_Network_PlayerMove(Card cardPlayed)
    //{
    //    if (GDFRNetworkManager.Instance.IsLocalClientTheHost())
    //    {
    //        GDFRNetworkManager.Instance.TriggerEvent(MsgIndexes.CardPlayed, new CardPlayedMessage(cardPlayed.Id));
    //    }

    //    //Get the select card from the event data;
    //    selectedCard = playedCard = cardPlayed;
    //    playerDecks[mPlayersPosition[currentPlayer]].DeckUiEnabled(false);

    //    EventReceiver.TriggerCardPlayedEvent(playedCard);

    //    yield return StartCoroutine(playedCard.AnimateDrawCard(playedCardDeck, 1.5f));
    //}

    private void NetOnDrawCard(NetworkMessage message)
    {
        //phase2Data.Add(message.ReadMessage<DrawCardMessage>());

        // DrawCardMessage mess = message.ReadMessage<DrawCardMessage>();

        // Deck fromDeck = mController.GetDeckFromId(mess.fromDeckId);
        // Deck toDeck = mController.GetDeckFromId(mess.toDeckId);
        // Card card = fromDeck.DrawExactCard(mess.cardId);

        // StartCoroutine(Phase1CardDraw(card, toDeck));
    }


    private void NetOnGroupDrawMessage(NetworkMessage message)
    {
        Debug.Log("getting group draw data");

        //cache command until we need it
        GroupedDrawMessage data = message.ReadMessage<GroupedDrawMessage>();

        switch (data.groupPhase)
        {
            case GroupDrawPhase.Phase1:
                phase1DrawData = data;
                break;
            case GroupDrawPhase.Phase2:
                phase2DrawData = data;
                break;
            case GroupDrawPhase.Phase3:
                phase3DrawData = data;
                break;
        }
    }

    private void NetOnRecieveInitiative(NetworkMessage message)
    {
        Debug.Log("getting data");

        IntMessage data = message.ReadMessage<IntMessage>();

        initiativeData = data;
    }

    private void NetOnCardPlayed(NetworkMessage message)
    {
        Debug.Log("getting data");

        CardPlayedMessage mess = message.ReadMessage<CardPlayedMessage>();

        Card card = mController.playerDecks[mController.PlayersPosition[mController.currentPlayer]].GetExactCard(mess.cardPlayed);

        mController.callEvent("CardPicked", card);
    }

    private void NetOnStartNewGame(NetworkMessage message)
    {
        phase1DrawData = null;
        phase2DrawData = null;
        phase3DrawData = null;
        initiativeData = null;

        StartCoroutine(mController.State_Offline_StartNewGame());
    }
}