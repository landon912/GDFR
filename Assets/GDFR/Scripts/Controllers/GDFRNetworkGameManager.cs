using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

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
    private GroupedDrawMessage phase1Data;
    private GroupedDrawMessage phase2Data;
    private GroupedDrawMessage phase3Data;

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
    }

    public void SetupServerMessageHandlers()
    {
        //NetworkServer.RegisterHandler(MsgType.Ready, NetOnClientReady);
    }

    private void OnDisable()
    {
        //GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.DrawCard);
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.GroupedDrawMessage);

        //NetworkServer.UnregisterHandler(MsgType.Ready);
    }

    public IEnumerator State_Network_DrawPhase1()
    {
        Debug.Log("NETWORK: " + " Player " + mController.currentPlayer + "- Position: " + mController.PLayersPosition[mController.currentPlayer] + " - State: DrawPhase1");

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
        while (phase1Data == null)
        {
            Debug.Log("Waiting for data");
            yield return null;
        }

        //handle data once we get it from server
        Deck fromDeck = mController.GetDeckFromId(phase1Data.fromDeck);

        for (int i = 0; i < phase1Data.cardIds.Length; i++)
        {
            Card c = fromDeck.DrawExactCard(phase1Data.cardIds[i]);
            Deck toDeck = mController.GetDeckFromId(phase1Data.toDeckIds[i]);
            yield return StartCoroutine(c.AnimateDrawCard(toDeck, mController.dealSpeed));
        }
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
        while (phase2Data == null)
        {
            Debug.Log("Waiting for data in phase2");
            yield return null;
        }


        Deck fromDeck = mController.GetDeckFromId(phase2Data.fromDeck);
        for (int i = 0; i < phase2Data.cardIds.Length; i++)
        {
            Card c = fromDeck.DrawExactCard(phase2Data.cardIds[i]);
            Deck toDeck = mController.GetDeckFromId(phase2Data.toDeckIds[i]);
            yield return StartCoroutine(c.AnimateDrawCard(toDeck, mController.dealSpeed));
        }
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
                //card.CurrentRace = Toolbox.Instance.gameSettings.RulesVariant == GameSettings.RulesVariantType.GoblinsRule ? Race.Goblin : Race.Fairy;

                cards.Add(card.Id);
                toDecks.Add(mController.fairyRingDeck.Id);

                //yield return StartCoroutine(card.AnimateDrawCard(fairyRingDeck, dealSpeed));
            }

            GroupedDrawMessage outMess = new GroupedDrawMessage(GroupDrawPhase.Phase3, mController.mainDeck.Id, cards, toDecks);

            GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.GroupedDrawMessage, outMess);
        }

        while(phase3Data == null)
        {
            Debug.Log("waiting for data in phase3");
            yield return null;
        }

        //todo: make this into a method and abstract it out of all three phases
        Deck fromDeck = mController.GetDeckFromId(phase3Data.fromDeck);
        for (int i = 0; i < phase3Data.cardIds.Length; i++)
        {
            Card c = fromDeck.DrawExactCard(phase3Data.cardIds[i]);
            c.CurrentRace = Toolbox.Instance.gameSettings.RulesVariant == GameSettings.RulesVariantType.GoblinsRule ? Race.Goblin : Race.Fairy;
            Deck toDeck = mController.GetDeckFromId(phase3Data.toDeckIds[i]);
            yield return StartCoroutine(c.AnimateDrawCard(toDeck, mController.dealSpeed));
        }

        mController.fairyRingDeck.Refresh();
    }

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
        //cache command until we need it
        GroupedDrawMessage data = message.ReadMessage<GroupedDrawMessage>();

        switch (data.groupPhase)
        {
            case GroupDrawPhase.Phase1:
                phase1Data = data;
                break;
            case GroupDrawPhase.Phase2:
                phase2Data = data;
                break;
            case GroupDrawPhase.Phase3:
                phase3Data = data;
                break;
        }
    }
}