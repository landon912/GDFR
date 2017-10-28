using UnityEngine;
using System.Collections;

public class nParticleRoot : MonoBehaviour {

	public static Transform worldTransform = null;

	// Use this for initialization
	void Start () {
		worldTransform = transform;
	}
}
