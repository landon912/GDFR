using UnityEngine;
using System.Collections;

public class nInitVelocity : MonoBehaviour {

	public virtual Vector3 GetVelocity(ref Vector3 position)
	{
		return Vector3.zero;
	}
}
