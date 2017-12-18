using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIModule
{
    public abstract int GetPlayValue(Card pCard, Deck toDeck, List<Deck> playerControlledDecks);

    public Card PickBestCard(Deck fromDeck, Deck toDeck, List<Deck> playerControlledDecks)
    {
        //grab the first one.  Don't want to return null;
        Card currentBestCard = null;
        //start each fromDeck card
        int bestDiscardValue = -10;
        Card[] pCards = fromDeck.GetCardList();
        foreach (Card pCard in pCards)
        {
            int discardValue = GetPlayValue(pCard, toDeck, playerControlledDecks);

            if (discardValue > bestDiscardValue)
            {
                currentBestCard = pCard;
                bestDiscardValue = discardValue;
            }
        }
        return currentBestCard;
    }

    //TODO: REMOVING GOBLINS SHOULD NOT BE A PLUS WHEN THE CARD CREATED THE GOBLIN IN THE FIRST PLACE(Rhyme or star card)
    public int CalculateAidToPlayers(List<Deck> playerDecks, Card cardBeingPlayed, Deck centerDeck)
    {
        if (Toolbox.Instance.gameSettings.rulesVariant == GameSettings.RulesVariant.Goblins_Rule)
        {
            return GoblinsRuleCalculateAidToPlayers(playerDecks, cardBeingPlayed, centerDeck);
        }

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

        return CalculateDirectEffects(centerCards, cardBeingPlayed, allPlayerCards) +
               CalculatePassiveEffects(centerCards, cardBeingPlayed, allPlayerCards);
    }

    private int CalculateDirectEffects(Card[] centerCards, Card cardBeingPlayed, List<Card> allPlayerCards)
    {
        int aidToPlayers = 0;

        //calculate direct effects
        foreach (Card centerCard in centerCards)
        {
            Symbol centerSymbol;
            Race centerRace;
            CheckAndFlipCardIfNeeded(centerCard, cardBeingPlayed, out centerRace, out centerSymbol);

            bool flipped = centerCard.CurrentRhyme == cardBeingPlayed.CurrentRhyme || cardBeingPlayed.StarsShowing;

            //would be able to take, CONSIDERING FLIP
            if (centerSymbol == cardBeingPlayed.CurrentSymbol)
            {
                foreach (Card playerCard in allPlayerCards)
                {
                    //player is trying to get rid of this card, would the current match help or hurt player??
                    if (playerCard.CurrentSymbol == centerSymbol && playerCard.CurrentRace == Race.Goblin)
                    {
                        //account for flipped state vs CURRENT state (we should evaluate aid based on current state)
                        if (!flipped)
                        {
                            //center card is goblin
                            if (centerRace == Race.Goblin)
                            {
                                //player card matches this goblin and the player card itself is a goblin, this is a problem for the player, we can remove and help them out
                                aidToPlayers++;
                                Debug.Log("Would remove a goblin of symbol " + centerSymbol +
                                          " from the middle which would help with player's " + playerCard);
                            }
                            //center card is fairy
                            else
                            {
                                aidToPlayers--;
                                Debug.Log("Would remove a fairy of symbol " + centerSymbol +
                                          " from the middle which would hurt with player's " + playerCard);
                            }
                        }
                        //cards have flipped, their actual game state if the current card is not played would be the opposite
                        else
                        {
                            //center card is ACTUALLY Goblin before flip; this is what we should consider
                            if (centerCard.CurrentRace == Race.Goblin)
                            {
                                //player card matches this goblin and the player card itself is a goblin, this is a problem for the player, we can remove and help them out
                                aidToPlayers++;
                                Debug.Log("Would remove a goblin of symbol " + centerCard.CurrentSymbol +
                                          " from the middle which would help with player's " + playerCard);
                            }
                            //center card is ACTUALLY Fairy, before flip; this is what we should consider
                            else
                            {
                                aidToPlayers--;
                                Debug.Log("Would remove a fairy of symbol " + centerCard.CurrentSymbol +
                                          " from the middle which would hurt with player's " + playerCard);
                            }
                        }
                    }
                }
            }
        }

        return aidToPlayers;
    }

    private int CalculatePassiveEffects(Card[] centerCards, Card cardBeingPlayed, List<Card> allPlayerCards)
    {
        int aidToPlayers = 0;

        //calculate passive effects
        foreach (Card playerCard in allPlayerCards)
        {
            //calculate before vs after effect on the entire deck considering flip or rhyme
            foreach (Card centerCard in centerCards)
            {
                bool wouldFlip = centerCard.CurrentRhyme == cardBeingPlayed.CurrentRhyme ||
                                 cardBeingPlayed.StarsShowing;

                if (wouldFlip)
                {
                    //check current card status
                    if (!playerCard.StarsShowing && playerCard.CurrentSymbol == centerCard.CurrentSymbol)
                    {
                        //center card is currently of use to player before flip
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Fairy)
                        {
                            Debug.Log(centerCard + " is currently of use to player before flip when compared to " + playerCard + "; taking away one aid point");
                            aidToPlayers--;
                        }
                        //center card is currently bad for the player before the flip
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Goblin)
                        {
                            Debug.Log(centerCard + " is currently bad for the player before flip when compared to " + playerCard + "; adding one aid point");
                            aidToPlayers++;
                        }
                    }
                    //if the player card is a star card or would rhyme with the center card if played, flip our evaluation
                    if ((playerCard.StarsShowing || centerCard.CurrentRhyme == playerCard.CurrentRhyme) &&
                        playerCard.CurrentSymbolGroup == centerCard.CurrentSymbolGroup && playerCard.CurrentSymbol != centerCard.CurrentSymbol)
                    {
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Goblin)
                        {
                            Debug.Log(centerCard + " is currently of use to player before flip when compared to " + playerCard + "; taking away one aid point");
                            aidToPlayers--;
                        }
                        if (playerCard.CurrentRace == Race.Goblin && centerCard.CurrentRace == Race.Fairy)
                        {
                            Debug.Log(centerCard + " is currently bad for the player before flip when compared to " + playerCard + "; adding one aid point");
                            aidToPlayers++;
                        }
                    }

                    Symbol centerSymbol;
                    Race centerRace;
                    Rhyme centerRhyme;
                    //actually flip now
                    CheckAndFlipCardIfNeeded(centerCard, cardBeingPlayed, out centerRace, out centerSymbol, out centerRhyme);


                    //if the player card is a star card or would rhyme with the center card if played, flip our evaluation
                    if (playerCard.StarsShowing || centerRhyme == playerCard.CurrentRhyme)
                    {
                        //would take
                        if (playerCard.CurrentSymbolGroup == centerCard.CurrentSymbolGroup && playerCard.CurrentSymbol != centerSymbol)
                        {
                            //since player card is a star card or rhyme
                            if (playerCard.CurrentRace == Race.Goblin && centerRace == Race.Goblin)
                            {
                                Debug.Log(centerCard + " would be good for the player after flip when compared to " + playerCard + "; taking away one aid point");
                                aidToPlayers++;
                            }
                            if (playerCard.CurrentRace == Race.Goblin && centerRace == Race.Fairy)
                            {
                                Debug.Log(centerCard + " would be bad for the player after flip when compared to " + playerCard + "; adding one aid point");
                                aidToPlayers--;
                            }
                        }
                    }
                    //check effects after flip normally, would be able to take
                    else if (playerCard.CurrentSymbol == centerSymbol)
                    {
                        //center card is now of use to player after flip
                        if (playerCard.CurrentRace == Race.Goblin && centerRace == Race.Fairy)
                        {
                            Debug.Log(centerCard + " would be good for the player after flip when compared to " + playerCard + "; adding one aid point");
                            aidToPlayers++;
                        }
                        //center card is now bad for the player after the flip
                        if (playerCard.CurrentRace == Race.Goblin && centerRace == Race.Goblin)
                        {
                            Debug.Log(centerCard + " would be bad for the player after flip when compared to " + playerCard + "; taking one aid point");
                            aidToPlayers--;
                        }
                    }
                }
            }

            //Debug.Log("After accounting for rhymes and star cards in comparison to " + playerCard + ", the aidValue is " + aidToPlayers + " which is a change of " + (aidToPlayers - tempOldAidValue));
            //tempOldAidValue = aidToPlayers;

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

    //TODO: REMOVING FAIRIES SHOULD NOT BE A PLUS WHEN THE CARD CREATED THE FAIRY IN THE FIRST PLACE(Rhyme or star card)
    public int GoblinsRuleCalculateAidToPlayers(List<Deck> playerDecks, Card cardBeingPlayed, Deck centerDeck)
    {
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

        return GoblinsRuleCalculateDirectEffects(centerCards, cardBeingPlayed, allPlayerCards) +
               GoblinsRuleCalculatePassiveEffects(centerCards, cardBeingPlayed, allPlayerCards);
    }

    private int GoblinsRuleCalculateDirectEffects(Card[] centerCards, Card cardBeingPlayed, List<Card> allPlayerCards)
    {
        int aidToPlayers = 0;

        //calculate direct effects
        foreach (Card centerCard in centerCards)
        {
            Symbol centerSymbol;
            Race centerRace;
            CheckAndFlipCardIfNeeded(centerCard, cardBeingPlayed, out centerRace, out centerSymbol);

            bool flipped = centerCard.CurrentRhyme == cardBeingPlayed.CurrentRhyme || cardBeingPlayed.StarsShowing;

            //would be able to take, CONSIDERING FLIP
            if (centerSymbol == cardBeingPlayed.CurrentSymbol)
            {
                foreach (Card playerCard in allPlayerCards)
                {
                    //player is trying to get rid of this card, would the current match help or hurt player??
                    if (playerCard.CurrentSymbol == centerSymbol && playerCard.CurrentRace == Race.Fairy)
                    {
                        //account for flipped state vs CURRENT state (we should evaluate aid based on current state)
                        if (!flipped)
                        {
                            //center card is goblin
                            if (centerRace == Race.Fairy)
                            {
                                //player card matches this fairy and the player card itself is a fairy, this is a problem for the player, we can remove and help them out
                                aidToPlayers++;
                                Debug.Log("Would remove a fairy of symbol " + centerSymbol +
                                          " from the middle which would help with player's " + playerCard);
                            }
                            //center card is goblin
                            else
                            {
                                aidToPlayers--;
                                Debug.Log("Would remove a goblin of symbol " + centerSymbol +
                                          " from the middle which would hurt with player's " + playerCard);
                            }
                        }
                        //cards have flipped, their actual game state if the current card is not played would be the opposite
                        else
                        {
                            //center card is ACTUALLY fairy before flip; this is what we should consider
                            if (centerCard.CurrentRace == Race.Fairy)
                            {
                                //player card matches this fairy and the player card itself is a fairy, this is a problem for the player, we can remove and help them out
                                aidToPlayers++;
                                Debug.Log("Would remove a fairy of symbol " + centerCard.CurrentSymbol +
                                          " from the middle which would help with player's " + playerCard);
                            }
                            //center card is ACTUALLY goblin, before flip; this is what we should consider
                            else
                            {
                                aidToPlayers--;
                                Debug.Log("Would remove a goblin of symbol " + centerCard.CurrentSymbol +
                                          "which would hurt with player's " + playerCard);
                            }
                        }
                    }
                }
            }
        }

        return aidToPlayers;
    }

    private int GoblinsRuleCalculatePassiveEffects(Card[] centerCards, Card cardBeingPlayed, List<Card> allPlayerCards)
    {
        int aidToPlayers = 0;

        //calculate passive effects
        foreach (Card playerCard in allPlayerCards)
        {
            //calculate before vs after effect on the entire deck considering flip or rhyme
            foreach (Card centerCard in centerCards)
            {
                bool wouldFlip = centerCard.CurrentRhyme == cardBeingPlayed.CurrentRhyme ||
                                 cardBeingPlayed.StarsShowing;

                if (wouldFlip)
                {
                    //check current card status
                    if (!playerCard.StarsShowing && playerCard.CurrentSymbol == centerCard.CurrentSymbol)
                    {
                        //center card is currently of use to player before flip
                        if (playerCard.CurrentRace == Race.Fairy && centerCard.CurrentRace == Race.Goblin)
                        {
                            Debug.Log(centerCard + " is currently of use to player before flip when compared to " + playerCard + "; taking away one aid point");
                            aidToPlayers--;
                        }
                        //center card is currently bad for the player before the flip
                        if (playerCard.CurrentRace == Race.Fairy && centerCard.CurrentRace == Race.Fairy)
                        {
                            Debug.Log(centerCard + " is currently bad for the player before flip when compared to " + playerCard + "; adding one aid point");
                            aidToPlayers++;
                        }
                    }
                    //if the player card is a star card or would rhyme with the center card if played, flip our evaluation
                    if ((playerCard.StarsShowing || centerCard.CurrentRhyme == playerCard.CurrentRhyme) &&
                        playerCard.CurrentSymbolGroup == centerCard.CurrentSymbolGroup && playerCard.CurrentSymbol != centerCard.CurrentSymbol)
                    {
                        if (playerCard.CurrentRace == Race.Fairy && centerCard.CurrentRace == Race.Fairy)
                        {
                            Debug.Log(centerCard + " is currently of use to player before flip when compared to " + playerCard + "; taking away one aid point");
                            aidToPlayers--;
                        }
                        if (playerCard.CurrentRace == Race.Fairy && centerCard.CurrentRace == Race.Goblin)
                        {
                            Debug.Log(centerCard + " is currently bad for the player before flip when compared to " + playerCard + "; adding one aid point");
                            aidToPlayers++;
                        }
                    }

                    Symbol centerSymbol;
                    Race centerRace;
                    Rhyme centerRhyme;
                    //actually flip now
                    CheckAndFlipCardIfNeeded(centerCard, cardBeingPlayed, out centerRace, out centerSymbol, out centerRhyme);


                    //if the player card is a star card or would rhyme with the center card if played, flip our evaluation
                    if (playerCard.StarsShowing || centerRhyme == playerCard.CurrentRhyme)
                    {
                        //would take
                        if (playerCard.CurrentSymbolGroup == centerCard.CurrentSymbolGroup && playerCard.CurrentSymbol != centerSymbol)
                        {
                            //since player card is a star card or rhyme
                            if (playerCard.CurrentRace == Race.Fairy && centerRace == Race.Fairy)
                            {
                                Debug.Log(centerCard + " would be good for the player after flip when compared to " + playerCard + "; taking away one aid point");
                                aidToPlayers++;
                            }
                            if (playerCard.CurrentRace == Race.Fairy && centerRace == Race.Goblin)
                            {
                                Debug.Log(centerCard + " would be bad for the player after flip when compared to " + playerCard + "; adding one aid point");
                                aidToPlayers--;
                            }
                        }
                    }
                    //check effects after flip normally, would be able to take
                    else if (playerCard.CurrentSymbol == centerSymbol)
                    {
                        //center card is now of use to player after flip
                        if (playerCard.CurrentRace == Race.Fairy && centerRace == Race.Goblin)
                        {
                            Debug.Log(centerCard + " would be good for the player after flip when compared to " + playerCard + "; adding one aid point");
                            aidToPlayers++;
                        }
                        //center card is now bad for the player after the flip
                        if (playerCard.CurrentRace == Race.Fairy && centerRace == Race.Fairy)
                        {
                            Debug.Log(centerCard + " would be bad for the player after flip when compared to " + playerCard + "; taking one aid point");
                            aidToPlayers--;
                        }
                    }
                }
            }

            Symbol cardAddedToCenterSymbol;
            Race cardAddedToCenterRace;
            CheckAndFlipCardIfNeeded(cardBeingPlayed, playerCard, out cardAddedToCenterRace, out cardAddedToCenterSymbol);

            //account for any damage this card may add by being a fairy (relative to each player card, may actually be a goblin)
            if (cardAddedToCenterRace == Race.Fairy)
            {
                //would add a fairy that hurts the player
                if (playerCard.CurrentSymbol == cardAddedToCenterSymbol && playerCard.CurrentRace == Race.Fairy)
                {
                    aidToPlayers--;
                    Debug.Log("Would add a goblin of symbol " + cardAddedToCenterSymbol + " which would hurt with player's " + playerCard);
                }
            }
            //account for any aid this card may add by being a goblin
            else
            {
                //would add a goblin to the center deck that would help the player
                if (playerCard.CurrentSymbol == cardAddedToCenterSymbol && playerCard.CurrentRace == Race.Fairy)
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

    public void CheckAndFlipCardIfNeeded(Card cardToFlip, Card playerCard, out Race tRace, out Symbol tSymbol,
        out Rhyme tRhyme)
    {
        tRace = cardToFlip.CurrentRace;
        tSymbol = cardToFlip.CurrentSymbol;
        tRhyme = cardToFlip.CurrentRhyme;
        CheckAndFlipCardIfNeeded(cardToFlip, playerCard, out tRace, out tSymbol);

        if (cardToFlip.CurrentRhyme == playerCard.CurrentRhyme || playerCard.StarsShowing)
        {
            tRhyme = cardToFlip.CurrentRace == Race.Fairy ? cardToFlip.goblinRhyme : cardToFlip.fairyRhyme;
        }
    }
}