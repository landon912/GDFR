using UnityEngine;
using System.Collections;

public class nPhysics : MonoBehaviour {

	public virtual void Apply(ref Vector3 outPut)
	{
		if(!enabled)
			return;
	}

	void OnEnable()
	{
	}
}
