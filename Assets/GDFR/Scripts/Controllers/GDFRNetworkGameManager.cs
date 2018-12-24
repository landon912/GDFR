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
    private Phase1DrawMessage phase1Data;

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
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.Phase1Draw, NetOnPhase1DrawData);
    }

    public void SetupServerMessageHandlers()
    {
        //NetworkServer.RegisterHandler(MsgType.Ready, NetOnClientReady);
    }

    private void OnDisable()
    {
        //GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.DrawCard);
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.Phase1Draw);

        //NetworkServer.UnregisterHandler(MsgType.Ready);
    }

    public bool phase1Lock = false;

    public IEnumerator State_Network_DrawPhase1()
    {
        Debug.Log("NETWORK: " + " Player " + mController.currentPlayer + "- Position: " + mController.PLayersPosition[mController.currentPlayer] + " - State: DrawPhase1");

        mController.CreateStarDeck();

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
            Phase1DrawMessage outMess = new Phase1DrawMessage(mController.starDeck.Id, cards, toDecks);

            GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.Phase1Draw, outMess);

            //TODO: Handling moving onto next phase
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

    private void NetOnDrawCard(NetworkMessage message)
    {
        // DrawCardMessage mess = message.ReadMessage<DrawCardMessage>();

        // Deck fromDeck = mController.GetDeckFromId(mess.fromDeckId);
        // Deck toDeck = mController.GetDeckFromId(mess.toDeckId);
        // Card card = fromDeck.DrawExactCard(mess.cardId);

        // StartCoroutine(Phase1CardDraw(card, toDeck));
    }


    private void NetOnPhase1DrawData(NetworkMessage message)
    {
        //cache command until we need it
        phase1Data = message.ReadMessage<Phase1DrawMessage>();
    }
}