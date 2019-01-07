using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GDFR
{
    public class UITable : MonoBehaviour
    {
        private List<RectTransform> children = new List<RectTransform>();

        protected void BuildGrid(Transform contentParent)
        {
            for (int i = 0; i < contentParent.childCount; i++)
            {
                children.Add(contentParent.GetChild(i).GetComponent<RectTransform>());
            }

            int numCols = Mathf.FloorToInt(Mathf.Sqrt(children.Count));

            Vector2 itemSize = children[0].GetComponent<Image>().rectTransform.sizeDelta;

            int currentCol = 0;
            int currentRow = 0;
            for (int i = 0; i < children.Count; i++)
            {
                children[i].transform.localScale = Vector3.one;

                if (currentCol < numCols)
                {
                    children[i].anchoredPosition = new Vector2(currentCol * itemSize.x, currentRow * -itemSize.y);
                    currentCol++;
                }
                else
                {
                    currentRow++;
                    currentCol = 0;
                    children[i].anchoredPosition = new Vector2(currentCol * itemSize.x, currentRow * -itemSize.y);
                    currentCol++;
                }
            }

            int totalRow = currentRow + 1; //will always end on last row

            RectTransform localRect = GetComponent<RectTransform>();
            localRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, numCols * itemSize.x);
            localRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalRow * itemSize.y);
        }
    }
}