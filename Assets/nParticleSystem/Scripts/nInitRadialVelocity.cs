using UnityEngine;
using System.Collections;

public class nInitRadialVelocity : nInitVelocity {

	public float minAmount = 0.1f;
	public float maxAmount = 1f;

	public override Vector3 GetVelocity (ref Vector3 position)
	{
		Vector3 emitterOutwardVector =  position.normalized;
		return emitterOutwardVector * Random.Range(minAmount,maxAmount);
	}
}
