using System.Collections.Generic;
using UnityEngine;

namespace GDFR
{
    public class VeryHardAIModule : AIModule
    {
        private readonly HardAIModule mHardAI;

        public VeryHardAIModule()
        {
            mHardAI = new HardAIModule();
        }

        public override int GetPlayValue(Card pCard, Deck toDeck, List<Deck> playerControlledDecks)
        {
            //get base play value
            int discardValue = 4 * mHardAI.GetPlayValue(pCard, toDeck);
            Debug.Log(pCard + " 's discard value before evaluating aid is " + discardValue);

            //calculate "pain" to other players
            discardValue -= CalculateAidToPlayers(playerControlledDecks, pCard, toDeck);

            discardValue = Mathf.RoundToInt(discardValue / 5.0f);

            Debug.Log("---------------------------------------------------------------------- \n " + pCard +
                      "'s discard value was " + discardValue);

            return discardValue;
        }
    }
}