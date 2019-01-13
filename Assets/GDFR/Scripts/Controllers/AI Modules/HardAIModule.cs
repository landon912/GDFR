using System.Collections.Generic;

namespace GDFR
{
    public class HardAIModule : AIModule
    {
        public override int GetPlayValue(Card pCard, Deck toDeck, List<Deck> playerControlledDecks = null)
        {
            if (Toolbox.Instance.gameSettings.RulesVariant == GameSettings.RulesVariantType.GoblinsRule)
            {
                return GoblinsRuleGetPlayValue(pCard, toDeck);
            }

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
                        discardValue -= 1;
                    else
                        discardValue += 1;
                }
            }

            if (pCard.CurrentRace == Race.Goblin)
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

            if (pCard.CurrentRace == Race.Fairy) discardValue -= 1;

            return discardValue;
        }

        private int GoblinsRuleGetPlayValue(Card pCard, Deck toDeck)
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
                        discardValue += 1;
                    else
                        discardValue -= 1;
                }
            }

            if (pCard.CurrentRace == Race.Fairy)
            {
                // Best play is to discard fairies if it doesnt get more faries
                if (discardValue == 0)
                {
                    discardValue += 1;

                    // In solitaire, best play is to discard fairies without getting others
                    if (Toolbox.Instance.gameSettings.numberOfPlayers == 1)
                    {
                        discardValue += 3;
                    }
                }

                discardValue += 1;
            }

            if (pCard.CurrentRace == Race.Goblin) discardValue -= 1;

            return discardValue;
        }
    }
}