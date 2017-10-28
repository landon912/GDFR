using UnityEngine;
using System.Collections;



public class DeckGrid : MonoBehaviour {

	public enum HorizontalJustify {Left, Center, Right};
	public enum VerticalJustify {Top,Center,Bottom};

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

	public virtual void Refresh()
	{
		int count = transform.childCount;
		int t = 0;
		for(int c=0;c<count;c++)
		{
			transform.GetChild(c).localPosition = new Vector3(GetGridPosition(c).x,GetGridPosition(c).y,0f);
			t = c;
		}

		if(count<2)return;
		Vector2 bounds = GetGridBounds();
		transform.localPosition = new Vector3(-(bounds.x-cellWidth)/2,(bounds.y-cellHeight) /2);
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
		if(maxPerLine>0)
			return index / maxPerLine;
		else
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
