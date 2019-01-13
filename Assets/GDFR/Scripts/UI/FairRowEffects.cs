using UnityEngine;
using System.Collections;

namespace GDFR
{
    public class FairRowEffects : MonoBehaviour
    {

        public GameObject starEffect = null;

        // Use this for initialization
        void OnEnable()
        {
            EventReceiver.StarPlayedEvent += OnStarPlayedEvent;
        }

        void OnDisable()
        {
            EventReceiver.StarPlayedEvent -= OnStarPlayedEvent;
        }

        void OnStarPlayedEvent(Card card)
        {
            if (starEffect.activeSelf == true)
                return;
            StartCoroutine(CO_StarEffect());
        }

        IEnumerator CO_StarEffect()
        {
            starEffect.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            starEffect.SetActive(false);
        }
    }
}