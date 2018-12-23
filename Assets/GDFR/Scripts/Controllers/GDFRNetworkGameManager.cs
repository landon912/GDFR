using System;
using System.Collections;
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
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.DrawCard, NetOnDrawCard);
    }

    public void SetupServerMessageHandlers()
    {
        //NetworkServer.RegisterHandler(MsgType.Ready, NetOnClientReady);
    }

    private void OnDisable()
    {
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.DrawCard);

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
            // Give 1 star card for each player
            foreach (Deck pDeck in mController.playerDecks)
            {
                // Enabled player ?
                if (pDeck.enabled)
                {
                    Card card = mController.starDeck.DrawRandomCard();

                    if(phase1Lock == true)
                    {
                        //wait
                        yield return null;
                    }
                    else
                    {
                        //disbatch order to clients
                        GDFRNetworkManager.Instance.TriggerEventIfHost(MsgIndexes.DrawCard, new DrawCardMessage(card.parentDeck, card, pDeck));
                        phase1Lock = true;
                    }
                }
            }

            //TODO: Handling moving onto next phase
        }
    }

    private void NetOnDrawCard(NetworkMessage message)
    {
        DrawCardMessage mess = message.ReadMessage<DrawCardMessage>();

        Deck fromDeck = mController.GetDeckFromId(mess.fromDeckId);
        Deck toDeck = mController.GetDeckFromId(mess.toDeckId);
        Card card = fromDeck.DrawExactCard(mess.cardId);

        StartCoroutine(Phase1CardDraw(card, toDeck));
    }

    private IEnumerator Phase1CardDraw(Card card, Deck toDeck)
    {
        yield return StartCoroutine(card.AnimateDrawCard(toDeck, mController.dealSpeed));

        //release lock
        phase1Lock = false;
    }
}