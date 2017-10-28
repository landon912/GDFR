using UnityEngine;
using System.Collections;

public class nInitRangeRotation : nInitRotation {

	public float minRotation = 0;
	public float maxRotation = 360;

	public override float GetRotation ()
	{
		return Random.Range(minRotation,maxRotation);
	}
}
