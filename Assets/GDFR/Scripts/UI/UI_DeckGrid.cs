
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
			foreach(Transform tr in transform)
				tr.localPosition = Vector3.zero;
			return;
		}
		//Debug.Log("Refreshed");
		int count = transform.childCount;
		for(int c=0;c<count;c++)
		{
			Vector3 newPosition = new Vector3(GetGridPosition(c).x,GetGridPosition(c).y,0f);
			Card card = transform.GetChild(c).GetComponent<Card>();
			TweenPosition tp = transform.GetChild(c).gameObject.GetComponent<TweenPosition>();
			TweenScale ts = transform.GetChild(c).gameObject.GetComponent<TweenScale>();
			TweenRotation tr = transform.GetChild(c).gameObject.GetComponent<TweenRotation>();
			tp.from = transform.GetChild(c).localPosition;
			ts.from = transform.GetChild(c).localScale;
			tr.from = transform.GetChild(c).localEulerAngles;
			tp.to = newPosition;
			ts.to = new Vector3(1f,1f,1f);
			tr.to = Vector3.zero;
			tp.ResetToBeginning();
			ts.ResetToBeginning();
			tr.ResetToBeginning();
			tp.enabled = true;
			ts.enabled = true;
			tr.enabled = true;

            //sets card's depth in comparison with the others
            if (hJustify==HorizontalJustify.Right)
			{
				card.Depth = /*(int)getYindex(c) + */ c * OFFSET_PER_CARD;
			}
			else
		        card.Depth = /*(int)getYindex(c) + */ c * -OFFSET_PER_CARD;
		}

		if(positionTweener==null)
			positionTweener = gameObject.GetComponent<TweenPosition>();
		positionTweener.enabled = false;

		positionTweener.from = positionTweener.to = transform.localPosition; 
		Vector2 bounds = GetGridBounds();
		if(hJustify==HorizontalJustify.Center)
			positionTweener.to.x = -(bounds.x-cellWidth)/2;
		if(vJustify==VerticalJustify.Center)
			positionTweener.to.y = (bounds.y-cellHeight) /2;

		positionTweener.ResetToBeginning();
		positionTweener.enabled = true;
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
			//Debug.Log(this.transform.parent.name + " " + totalWidth);
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
