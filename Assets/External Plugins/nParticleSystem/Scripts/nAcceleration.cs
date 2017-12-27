using UnityEngine;
using System.Collections;

public class nAcceleration : nPhysics {

	public Vector3 acceleration = new Vector3(0f,-1f,0f);

	public override void Apply (ref Vector3 outPut)
	{
		if(!enabled)
			return;
		outPut+=acceleration;
	}
}
