using System.Collections.Generic;

public abstract class AIModule
{
    public abstract int GetPlayValue(Card pCard, Deck toDeck, int modifier, List<Deck> playerDecks);

    public int CheckIfWouldTakeFairiesFromPlayer(List<Deck> playerDecks, Card cardInCenterDeck, int modifier)
    {
        int count = 0;
        foreach (Deck playerDeck in playerDecks)
        {
            Card[] playerCards = playerDeck.GetCardList();
            foreach (Card playerCard in playerCards)
            {
                //if the player has goblins of this symbol, we want to leave them (fairies) to help; or based on the modifier(pos for fairies, neg for goblins), we want to take them.
                if (playerCard.CurrentSymbol == cardInCenterDeck.CurrentSymbol && cardInCenterDeck.CurrentRace == Race.Fairy && playerCard.CurrentRace == Race.Goblin)
                {
                    count -= 2 * modifier;
                }
            }
        }
        return count;
    }

    public int CheckIfWouldAddBadGoblinsToPlayer(List<Deck> playerDecks, Card cardBeingPlayed, int modifier)
    {
        int count = 0;
        foreach (Deck playerDeck in playerDecks)
        {
            Card[] playerCards = playerDeck.GetCardList();
            foreach (Card playerCard in playerCards)
            {
                //we do not want to add goblins of the same symbol type as goblins in the player's hand to the center
                if (playerCard.CurrentSymbol == cardBeingPlayed.CurrentSymbol && cardBeingPlayed.CurrentRace == Race.Goblin)
                {
                    count -= 2 * modifier;
                }
            }
        }
        return count;
    }

}