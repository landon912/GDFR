using System.Collections.Generic;
using UnityEngine;

public class EasyAIModule : AIModule
{
    private readonly HardAIModule mHardAI;

    public EasyAIModule()
    {
        mHardAI = new HardAIModule();
    }

    public override int GetPlayValue(Card pCard, Deck toDeck, int modifier, List<Deck> playerDecks)
    {
        ////half of the time || SKIP IF IN SOLITAIRE MODE
        //if (Random.Range(0, 2) == 0 || Toolbox.Instance.gameSettings.numberOfPlayers == 1)
        //{
        //    //make a "good" move
        //    //return new HardAIModule().GetPlayValue(pCard, toDeck, modifier);
        //}

        //get base play value
        int discardValue = mHardAI.GetPlayValue(pCard, toDeck, modifier);
        Debug.Log(pCard + " 's discard value before evaluating aid is " + discardValue);
        
        //calculate aid
        discardValue += 2 * CalculateAidToPlayers(playerDecks, pCard, toDeck, modifier);
        Debug.Log("---------------------------------------------------------------------- \n " + pCard + "'s aid value was " + 2 * discardValue);

        return discardValue;
    }
}