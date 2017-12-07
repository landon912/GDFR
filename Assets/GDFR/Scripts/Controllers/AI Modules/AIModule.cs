﻿using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIModule
{
    public abstract int GetPlayValue(Card pCard, Deck toDeck, int modifier, List<Deck> playerDecks);

    //public int CheckIfWouldTakeFairiesFromPlayer(List<Deck> playerDecks, Card cardInCenterDeck, int modifier)
    //{
    //    int count = 0;
    //    foreach (Deck playerDeck in playerDecks)
    //    {
    //        Card[] playerCards = playerDeck.GetCardList();
    //        foreach (Card playerCard in playerCards)
    //        {
    //            Symbol centerSymbol;
    //            Race centerRace;
    //            CheckAndFlipCardIfNeeded(cardInCenterDeck, playerCard, out centerRace, out centerSymbol);

    //            //if the player has goblins of this symbol, we want to leave them (fairies) to help; or based on the modifier(pos for fairies, neg for goblins), we want to take them.
    //            if (playerCard.CurrentSymbol == centerSymbol && centerRace == Race.Fairy && playerCard.CurrentRace == Race.Goblin)
    //            {
    //                count -= 2 * modifier;
    //            }
    //        }
    //    }
    //    return count;
    //}

    //public int CheckIfWouldAddBadGoblinsToPlayer(List<Deck> playerDecks, Card cardBeingPlayed, int modifier)
    //{
    //    int count = 0;
    //    foreach (Deck playerDeck in playerDecks)
    //    {
    //        Card[] playerCards = playerDeck.GetCardList();
    //        foreach (Card playerCard in playerCards)
    //        {
    //            Symbol centerSymbol;
    //            Race centerRace;
    //            CheckAndFlipCardIfNeeded(cardBeingPlayed, playerCard, out centerRace, out centerSymbol);

    //            //we do not want to add goblins of the same symbol type as goblins in the player's hand to the center
    //            if (playerCard.CurrentSymbol == centerSymbol && centerRace == Race.Goblin)
    //            {
    //                count -= 2 * modifier;
    //            }
    //        }
    //    }
    //    return count;
    //}

    //TODO: REMOVING GOBLINS SHOULD NOT BE A PLUS WHEN THE CARD CREATED THE GOBLIN IN THE FIRST PLACE(Rhyme or star card)
    public int CalculateAidToPlayers(List<Deck> playerDecks, Card cardBeingPlayed, Deck centerDeck, int modifier)
    {
        int tempOldAidValue = 0;

        int aidToPlayers = 0;

        //get all player cards
        List<Card> allPlayerCards = new List<Card>();
        foreach (Deck playerDeck in playerDecks)
        {
            Card[] playerCards = playerDeck.GetCardList();
            foreach (Card playerCard in playerCards)
            {
                allPlayerCards.Add(playerCard);
            }
        }

        Card[] centerCards = centerDeck.GetCardList();

        //calculate direct effects
        foreach (Card centerCard in centerCards)
        {
            Symbol centerSymbol;
            Race centerRace;
            CheckAndFlipCardIfNeeded(centerCard, cardBeingPlayed, out centerRace, out centerSymbol);

            //would be able to take
            if (centerSymbol == cardBeingPlayed.CurrentSymbol)
            {
                foreach (Card playerCard in allPlayerCards)
                {
                    //player is trying to get rid of this held card
                    if (playerCard.CurrentSymbol == centerSymbol && playerCard.CurrentRace == Race.Goblin)
                    {
                        //center card is goblin
                        if (centerRace == Race.Goblin)
                        {
                            //player card matches this goblin and the player card itself is a goblin, this is a problem for the player, we can remove and help them out
                            aidToPlayers++;
                            Debug.Log("Would remove a goblin of symbol " + centerSymbol +
                                      "which would help with player's " + playerCard);
                        }
                        //center card is fairy
                        else
                        {
                            aidToPlayers--;
                            Debug.Log("Would remove a fairy of symbol " + centerSymbol +
                                      "which would hurt with player's " + playerCard);
                        }
                    }
                }
            }
            
        }

        Debug.Log("After accounting for direct effects, the aidValue is " + aidToPlayers + " which is a change of " + (aidToPlayers-tempOldAidValue));
        tempOldAidValue = aidToPlayers;

        //calculate passive effects
        foreach (Card playerCard in allPlayerCards)
        {
            //calculate before vs after effect on the entire deck due to flip or rhyme
            foreach (Card centerCard in centerCards)
            {
                bool wouldFlip = centerCard.CurrentRhyme == cardBeingPlayed.CurrentRhyme ||
                                 cardBeingPlayed.StarsShowing;

                if (wouldFlip)
                {
                    //check current card status
                    if (playerCard.CurrentSymbol == centerCard.CurrentSymbol)
                    {
                        //center card is currently of use to player before flip
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Fairy)
                        {
                            aidToPlayers--;
                        }
                        //center card is currently bad for the player before the flip
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Goblin)
                        {
                            aidToPlayers++;
                        }
                    }

                    Symbol centerSymbol;
                    Race centerRace;
                    //actually flip now
                    CheckAndFlipCardIfNeeded(centerCard, playerCard, out centerRace, out centerSymbol);

                    //check effects after flip
                    if (playerCard.CurrentSymbol == centerSymbol)
                    {
                        //center card is now of use to player after flip
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Fairy)
                        {
                            aidToPlayers++;
                        }
                        //center card is now bad for the player after the flip
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Goblin)
                        {
                            aidToPlayers--;
                        }
                    }
                }
            }

            Debug.Log("After accounting for rhymes and star cards, the aidValue is " + aidToPlayers + " which is a change of " + (aidToPlayers - tempOldAidValue));
            tempOldAidValue = aidToPlayers;

            Symbol cardAddedToCenterSymbol;
            Race cardAddedToCenterRace;
            CheckAndFlipCardIfNeeded(cardBeingPlayed, playerCard, out cardAddedToCenterRace, out cardAddedToCenterSymbol);

            //account for any damage this card may add by being a goblin (relative to each player card, may actually be a fairy)
            if (cardAddedToCenterRace == Race.Goblin)
            {
                //would add a goblin that hurts the player
                if (playerCard.CurrentSymbol == cardAddedToCenterSymbol && playerCard.CurrentRace == Race.Goblin)
                {
                    aidToPlayers--;
                    Debug.Log("Would add a goblin of symbol " + cardAddedToCenterSymbol + " which would hurt with player's " + playerCard);
                }
            }
            //account for any aid this card may add by being a fairy
            else
            {
                //would add a fairy to the center deck that would help the player
                if (playerCard.CurrentSymbol == cardAddedToCenterSymbol && playerCard.CurrentRace == Race.Goblin)
                {
                    aidToPlayers++;
                    Debug.Log("Would add a fairy of symbol " + cardAddedToCenterSymbol + " which would help with player's " + playerCard);
                }
            }
        }

        return aidToPlayers;
    }

    public void CheckAndFlipCardIfNeeded(Card cardToFlip, Card playerCard, out Race tRace, out Symbol tSymbol)
    {
        tRace = cardToFlip.CurrentRace;
        tSymbol = cardToFlip.CurrentSymbol;
        if(cardToFlip.CurrentRhyme == playerCard.CurrentRhyme || playerCard.StarsShowing)
        {
            switch (cardToFlip.CurrentSymbol)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (cardToFlip.CurrentRace)
            {
                case Race.Fairy:
                    tRace = Race.Goblin;
                    break;
                case Race.Goblin:
                    tRace = Race.Fairy;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}