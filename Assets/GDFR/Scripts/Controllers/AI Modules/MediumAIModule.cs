using System.Collections.Generic;
using UnityEngine;

public class MediumAIModule : AIModule
{
    private readonly HardAIModule mHardAI;

    public MediumAIModule()
    {
        mHardAI = new HardAIModule();
    }

    public override int GetPlayValue(Card pCard, Deck toDeck, int modifier, List<Deck> playerDecks)
    {
        //get base play value
        int discardValue = 3 * mHardAI.GetPlayValue(pCard, toDeck, modifier);
        Debug.Log(pCard + " 's discard value before evaluating aid is " + discardValue);

        //calculate aid
        discardValue += 2 * CalculateAidToPlayers(playerDecks, pCard, toDeck, modifier);

        discardValue = Mathf.RoundToInt(discardValue / 5.0f);

        Debug.Log("---------------------------------------------------------------------- \n " + pCard + "'s discard value was " + discardValue);

        return discardValue;
    }
}