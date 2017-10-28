using UnityEngine;
using System.Collections;

public class nKiller : MonoBehaviour {

	public float lifespan = 1f;
	float startTime;
	public float age;
	float life;

	void Start()
	{
		startTime = Time.time;
	}

	void FixedUpdate()
	{
		//particle life
		life = Time.time - startTime;

		age = life/lifespan;

		//lifespan
		if(life > lifespan)
			Destroy(gameObject);
	}
}
