using UnityEngine;
using System.Collections;

public class nCreator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public virtual Vector3 getSpawnPosition()
	{
		return transform.position;
	}
}
