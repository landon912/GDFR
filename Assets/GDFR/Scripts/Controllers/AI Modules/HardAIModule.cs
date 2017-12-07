using System.Collections.Generic;

public class HardAIModule : AIModule
{
    public override int GetPlayValue(Card pCard, Deck toDeck, int modifier, List<Deck> playerDecks = null)
    {
        Card[] tCards = toDeck.GetCardList();
        int discardValue = 0;

        foreach (Card tCard in tCards)
        {
            // is it a rhyme or pCard = star?
            // invert symbol and race to make the calculation
            Symbol tSymbol;
            Race tRace;
            CheckAndFlipCardIfNeeded(tCard, pCard, out tRace, out tSymbol);

            //is it a match?
            if (tSymbol == pCard.CurrentSymbol)
            {
                if (tRace == Race.Goblin)
                    discardValue -= 1 * modifier;
                else
                    discardValue += 1 * modifier;
            }
        }

        if (pCard.CurrentRace == Race.Goblin)
        {
            // Best play is to discard goblins if it doesnt get more goblins
            if (discardValue == 0)
            {
                discardValue += 1 * modifier;

                // In solitaire, best play is to discard goblins without get others
                if (Toolbox.Instance.gameSettings.numberOfPlayers == 1)
                {
                    discardValue += 3 * modifier;
                }
            }
            discardValue += 1 * modifier;
        }

        if (pCard.CurrentRace == Race.Fairy) discardValue -= 1 * modifier;

        return discardValue;
    }
}