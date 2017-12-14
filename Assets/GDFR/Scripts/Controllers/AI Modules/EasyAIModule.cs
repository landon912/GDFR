﻿using System.Collections.Generic;
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
        //get base play value
        int discardValue = 2 * mHardAI.GetPlayValue(pCard, toDeck, modifier);
        Debug.Log(pCard + " 's discard value before evaluating aid is " + discardValue);
        
        //calculate aid
        discardValue += 3 * CalculateAidToPlayers(playerDecks, pCard, toDeck, modifier);

        discardValue = Mathf.RoundToInt(discardValue / 5.0f);
        Debug.Log("---------------------------------------------------------------------- \n " + pCard + "'s aid value was " + 2 * discardValue);

        return discardValue;
    }
}