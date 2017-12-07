using System.Collections.Generic;
using UnityEngine;

public class EasyAIModule : AIModule
{
    public override int GetPlayValue(Card pCard, Deck toDeck, int modifier, List<Deck> playerDecks)
    {
        int discardValue = 0;

        //half of the time || SKIP IF IN SOLITAIRE MODE
        if (Random.Range(0, 2) == 0 || Toolbox.Instance.gameSettings.numberOfPlayers == 1)
        {
            //make a "good" move
            //return new HardAIModule().GetPlayValue(pCard, toDeck, modifier);
        }

        //get base play value
        discardValue = new HardAIModule().GetPlayValue(pCard, toDeck, modifier);
        int temp = discardValue;
        Debug.Log(pCard + " 's discard value before evaluating aid is " + discardValue);

        discardValue += CalculateAidToPlayers(playerDecks, pCard, toDeck, modifier);
        Debug.Log(pCard + " 's discard value after evaluating aid is " + discardValue + "; aid contributed " + (discardValue-temp));

        //help the players

        //Card[] tCards = toDeck.GetCardList();

        //foreach (Card tCard in tCards)
        //{
        //    Debug.Log("Comparing card in AI deck -> " + pCard + " with card in center -> " + tCard);

        //    // is it a rhyme or pCard = star?
        //    // invert symbol and race to make the calculation
        //    Symbol tSymbol;
        //    Race tRace;
        //    CheckAndFlipCardIfNeeded(tCard, pCard, out tRace, out tSymbol);

        //    //is it a match?
        //    if (tSymbol == pCard.CurrentSymbol)
        //    {
        //        if (pCard.CurrentRace == Race.Goblin)
        //        {
        //            discardValue += CheckIfWouldTakeFairiesFromPlayer(playerDecks, tCard, modifier);
        //            Debug.Log("After checking if would take fairies from player, the discard value is " + discardValue + ". " +
        //                      "This step contributed to this value " + CheckIfWouldTakeFairiesFromPlayer(playerDecks, tCard, modifier));
        //        }
        //        //is a fairy; we want to leave this if the player has this symbol
        //        else
        //        {
        //            discardValue += CheckIfWouldTakeFairiesFromPlayer(playerDecks, tCard, modifier);
        //            Debug.Log("After checking if would take fairies from player, the discard value is " + discardValue + ". " +
        //                      "This step contributed to this value " + CheckIfWouldTakeFairiesFromPlayer(playerDecks, tCard, modifier));
        //        }
        //    }
        //}

        ////our card is a goblin
        //if (pCard.CurrentRace == Race.Goblin)
        //{
        //    //// we dont want to add any more goblins to the center deck of the symbol types the players have as goblins
        //    //if (discardValue == 0)
        //    //{
        //    //    discardValue += 1 * modifier;
        //    //    Debug.Log("Because we don't want to add any more goblins the discard value is now " + discardValue + ". " +
        //    //              "This step contributed to this value" + (1 * modifier));
        //    //}
        //    discardValue += CheckIfWouldAddBadGoblinsToPlayer(playerDecks, pCard, modifier) + modifier;
        //    Debug.Log("After checking if this card would add bad goblins to the player, the discard value is now " + discardValue + ". " +
        //              "This step contributed to this value" + (CheckIfWouldAddBadGoblinsToPlayer(playerDecks, pCard, modifier) + modifier));
        //}
        //else if (pCard.CurrentRace == Race.Fairy)
        //{
        //    discardValue -= 1 * modifier;
        //    Debug.Log("Since this card is a fairy we don't want to get rid of it unless it helps the player, the discard value is now " + discardValue + ". " +
        //              "This step contributed to this value" + -(1 * modifier));
        //}

        //Debug.Log("The final discard value for this card is " + discardValue);

        return discardValue;
    }
}