using UnityEngine;
using System.Collections;

public class NCreatorBox : nCreator {

	public Vector3 minRange = new Vector3(-0.5f,-0.5f,-0.5f);
	public Vector3 maxRange = new Vector3(0.5f,0.5f,0.5f);
	public bool hollow = false;
	float x,y,z;

	// Use this for initialization
	void Start () {
	
	}

	public override Vector3 getSpawnPosition ()
	{
		if(!hollow)
		{
			x = Random.Range(minRange.x,maxRange.x);
			y = Random.Range(minRange.x,maxRange.y);
			z = Random.Range(minRange.x,maxRange.z);
		}
		else
		{
			//hollow version not yet implemented.
		}
		return new Vector3(x,y,z);
	}

	void OnDrawGizmosSelected() {
			Gizmos.color = Color.blue;
			Vector3[] p = new Vector3[4];
			p[0] = transform.TransformPoint(new Vector3(minRange.x,minRange.y,0f));
			p[1] = transform.TransformPoint(new Vector3(maxRange.x,minRange.y,0f));
			p[2] = transform.TransformPoint(new Vector3(maxRange.x,maxRange.y,0f));
			p[3] = transform.TransformPoint(new Vector3(minRange.x,maxRange.y,0f));

		for(int s=0;s<3;s++)
			Gizmos.DrawLine(p[s], p[s+1]);
		Gizmos.DrawLine(p[3], p[0]);
	}
}
