using UnityEngine;
using System.Collections;

public class NCreatorSphere : nCreator {

	public float size = 1f;

	public override Vector3 getSpawnPosition ()
	{
		return Random.insideUnitSphere * size;
	}

	void OnDrawGizmosSelected() {
		//Gizmos.color = Color.blue;
		//Gizmos.DrawSphere(transform.position,size);
	}
}
