using System.Collections.Generic;
using UnityEngine;

namespace GDFR
{
    public class MediumAIModule : AIModule
    {
        private readonly HardAIModule mHardAI;

        public MediumAIModule()
        {
            mHardAI = new HardAIModule();
        }

        public override int GetPlayValue(Card pCard, Deck toDeck, List<Deck> playerControlledDecks)
        {
            //get base play value
            int discardValue = 3 * mHardAI.GetPlayValue(pCard, toDeck);
            Debug.Log(pCard + " 's discard value before evaluating aid is " + discardValue);

            //calculate aid
            discardValue += 2 * CalculateAidToPlayers(playerControlledDecks, pCard, toDeck);

            discardValue = Mathf.RoundToInt(discardValue / 5.0f);

            Debug.Log("---------------------------------------------------------------------- \n " + pCard +
                      "'s discard value was " + discardValue);

            return discardValue;
        }
    }
}