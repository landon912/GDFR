using System.Collections.Generic;
using UnityEngine;

public class EasyAIModule : AIModule
{
    public override int GetPlayValue(Card pCard, Deck toDeck, int modifier, List<Deck> playerDecks)
    {
        //half of the time || SKIP IF IN SOLITAIRE MODE
        if (Random.Range(0, 2) == 0 || Toolbox.Instance.gameSettings.numberOfPlayers == 1)
        {
            //make a "good" move
            //return new HardAIModule().GetPlayValue(pCard, toDeck, modifier);
        }

        //help the players

        int discardValue = 0;
        Card[] tCards = toDeck.GetCardList();

        foreach (Card tCard in tCards)
        {
            Symbol tSymbol = tCard.CurrentSymbol;
            Race tRace = tCard.CurrentRace;

            // is it a rhyme or pCard = star?
            // invert symbol and race to make the calculation
            if (tCard.CurrentRhyme == pCard.CurrentRhyme || pCard.StarsShowing)
            {
                switch (tCard.CurrentSymbol)
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
                switch (tCard.CurrentRace)
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
            if (tSymbol == pCard.CurrentSymbol)
            {
                if (tRace == Race.Goblin)
                {
                    // modifier*(-2) b/c we want these to have a heavy effect && they are goblins; HOWEVER, we still want to play if there are no obvious card to help with.
                    discardValue += CheckIfWouldTakeFairiesFromPlayer(playerDecks, tCard, modifier * -2) - modifier;
                }
                //is a fairy; we want to leave this if the player has this symbol
                else
                {
                    // modifier*(-2) b/c we want these to have a heavy effect && they are fairies; HOWEVER, we still want to play appropriatly if there are no obvious card to help with.
                    discardValue += CheckIfWouldTakeFairiesFromPlayer(playerDecks, tCard, modifier * 2) + modifier;
                }
            }
        }

        //our card is a goblin
        if (pCard.CurrentRace == Race.Goblin)
        {
            // we dont want to add any more goblins to the center deck of the symbol types the players have as goblins
            if (discardValue == 0)
            {
                discardValue += 1 * modifier;
            }
            discardValue += CheckIfWouldAddBadGoblinsToPlayer(playerDecks, pCard, modifier * 2) + modifier;
        }

        if (pCard.CurrentRace == Race.Fairy) discardValue -= 1 * modifier;

        return discardValue;
    }
}