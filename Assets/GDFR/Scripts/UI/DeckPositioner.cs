using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDFR
{
    public class DeckPositioner : MonoBehaviour
    {
        public enum HorizontalOrder { LeftToR, Center, RightToL };

        public Vector2 elementSize;
        public int maxWidth;
        public HorizontalOrder hOrder;

        private RectTransform mLocalRectTransform; //do not use directly

        private RectTransform LocalRectTransform
        {
            get
            {
                if (mLocalRectTransform == null)
                {
                    mLocalRectTransform = GetComponent<RectTransform>();
                }

                return mLocalRectTransform;
            }
        }

        public void SetPosition()
        {
            const float tweenSpeed = 0.33f;

            int total = LocalRectTransform.childCount;
            for (int i = 0; i < total; i++)
            {
                Card card = LocalRectTransform.GetChild(i).GetComponent<Card>();

                Vector3 to = GetGridPosition(i, total);

                //reset
                LeanTween.move(card.LocalRectTransform, to, tweenSpeed);
                LeanTween.rotateLocal(card.gameObject, Vector3.zero, tweenSpeed);
                LeanTween.scale(card.LocalRectTransform, Vector3.one, tweenSpeed);

                //sets card's depth in comparison with the others
                card.Depth = hOrder == HorizontalOrder.LeftToR ? i : -i;
            }
        }

        private Vector3 GetGridPosition(int index, int total)
        {
            float x, y;
            float sFactor = 1.0f;
            float totalWidth = total * elementSize.x;

            if (totalWidth > maxWidth)
            {
                sFactor = maxWidth / totalWidth;
            }

            float sWidth = (elementSize.x * sFactor);
            float startLoc = 0;

            float rawLoc = 0f;

            switch (hOrder)
            {
                case HorizontalOrder.LeftToR:
                    rawLoc -= sWidth / 3.0f;
                    break;
                case HorizontalOrder.Center:
                    startLoc = /*(sWidth / 2.0f)*/ - (totalWidth * sFactor) / 2.0f;
                    break;
                case HorizontalOrder.RightToL:
                    rawLoc += sWidth / 3.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rawLoc += startLoc + index * sWidth;

            return new Vector3(rawLoc, 0, 0);
        }
    }
}