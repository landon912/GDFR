
//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;


public class UI_DeckGrid : UIWidgetContainer
{

	public enum HorizontalJustify {Left, Center, Right};
	public enum VerticalJustify {Top,Center,Bottom};

	TweenPosition positionTweener;

	public int maxPerLine = 10;
	public float _cellWidth = 200;
	public float cellHeight
	{
		set
		{
			cellHeight = value;
		}
		get{return _cellHeight * limitFactor;}
	}
	public float _cellHeight = 300;
	public float cellWidth
	{
		set
		{
			_cellWidth = value;
		}
		get{return _cellWidth * limitFactor;}
	}
	public float widthLimit = 1000f;
	public bool limitWidth = false;
	public HorizontalJustify hJustify = HorizontalJustify.Center;
	public VerticalJustify vJustify = VerticalJustify.Center;
	public Vector3 newRowOffset;
	public float limitFactor = 1f;
	int cellCount
	{
		get{return transform.childCount;}
	}

    private const int OFFSET_PER_CARD = 6;
    public void Reposition()
	{
		if(!enabled)
		{
			foreach(RectTransform tr in transform)
				tr.anchoredPosition3D = Vector3.zero;
			return;
		}

        const float TWEEN_SPEED = 0.33f;

		int count = transform.childCount;
		for(int c=0;c<count;c++)
		{
			Card card = transform.GetChild(c).GetComponent<Card>();

            Vector3 to = new Vector3(GetGridPosition(c).x, GetGridPosition(c).y, 0f);

            LeanTween.move(card.LocalRectTransform, to, TWEEN_SPEED);
            LeanTween.rotateLocal(card.gameObject, Vector3.zero, TWEEN_SPEED);
            LeanTween.scale(card.LocalRectTransform, Vector3.one, TWEEN_SPEED);

            //sets card's depth in comparison with the others
            if (hJustify==HorizontalJustify.Right)
			{
				card.Depth = c * OFFSET_PER_CARD;
			}
			else
		        card.Depth = c * -OFFSET_PER_CARD;
		}

        Vector2 bounds = GetGridBounds();
        RectTransform localRect = GetComponent<RectTransform>();

        float x = localRect.anchoredPosition.x;
        float y = localRect.anchoredPosition.y;
        if(hJustify == HorizontalJustify.Center)
        {
            x = -(bounds.x - cellWidth) / 2;
        }
        if(vJustify == VerticalJustify.Center)
        {
            y = (bounds.y - cellHeight) / 2;
        }

        Vector3 toPos = new Vector3(x, y, localRect.anchoredPosition3D.z);

        LeanTween.move(GetComponent<RectTransform>(), toPos, TWEEN_SPEED);
	}

	public Vector2 GetGridBounds()
	{
		float x = 0f,y = 0f;
		int count = transform.childCount;
		if(maxPerLine>0)
		{
			if(count >= maxPerLine)
			{
				x = cellWidth * maxPerLine;
			}
			else
				x = cellWidth * (count%maxPerLine);
			y = (((count-1) / maxPerLine)+1)  * cellHeight;
		}
		else
		{
			x = cellWidth * count;
			y = cellHeight;
		}
		return new Vector2(x,y);
	}

	public float getYindex(int index)
	{
		//if(maxPerLine>0)
			//return index / (float)maxPerLine;
		//else
			return 0f;
	}

	public virtual Vector3 GetGridPosition(int index)
	{
		float x = 0f,y = 0f;
		float tempy = 0f;
		if(maxPerLine>0)
		{
			x = cellWidth * (index%maxPerLine);
			tempy = getYindex(index);
			y = -(tempy * cellHeight);
			//add offset
			x += newRowOffset.x * tempy;
			y += newRowOffset.y * tempy;
		}
		float totalWidth = _cellWidth * cellCount;
		if(totalWidth>=widthLimit && limitWidth)
		{
			limitFactor = widthLimit/totalWidth;
		}
		else
			limitFactor = 1f;
		if(hJustify==HorizontalJustify.Left || hJustify==HorizontalJustify.Right)
			x+=_cellWidth/2;
		if(vJustify==VerticalJustify.Top)
			y-=_cellHeight/2;
		if(vJustify==VerticalJustify.Bottom)
			y+=_cellHeight/2;
		if(hJustify==HorizontalJustify.Right)
			x*=-1f;
		return new Vector3(x,y,0f);
	}
}