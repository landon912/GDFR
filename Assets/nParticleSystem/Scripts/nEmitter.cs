using UnityEngine;
using System.Collections;

public class nEmitter : MonoBehaviour {

	public nParticle particle = null;
	public Transform renderTransform = null;
	public float pps = 10;
	//public Vector2 startSizeRange = new Vector2(0,1);
	//public Vector2 starSpeedRange = new Vector2(0,1);
	public Vector2 lifeSpanRange = new Vector2(1,3);
	public float startTime = 0f;
	public float stopTime = -1f;
	Vector3 spawnP;
	float spawnInterval;
	float nextSpawnTime = 0;
	float _time;
	Vector3 position;
	Vector3 tempSphere;
	nCreator creator;
	float emitterEnableTime;
	nInitSize initSize;
	float _startSize = 1f;
	nInitVelocity initVelocity;
	Vector3 _startVelocity = Vector3.zero;
	nInitRotation initRotation;
	float _startRotation = 0f;

	// Use this for initialization
	void Awake(){
		creator = GetComponent<nCreator>();
		initSize = GetComponent<nInitSize>();
		initVelocity = GetComponent<nInitVelocity>();
		initRotation = GetComponent<nInitRotation>();

	}

	void Start () {
		spawnInterval = 1/pps;
		if(initSize!=null)
			_startSize = initSize.size;
		if(renderTransform==null)
			renderTransform = nParticleRoot.worldTransform;
	}

	void OnEnable()
	{
		emitterEnableTime = Time.time;
		nextSpawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(creator==null || !creator.enabled)return;
		if(stopTime>0)
			if((Time.time - emitterEnableTime)>=stopTime)
				enabled = false;
		while(Time.time>=nextSpawnTime)
		{
			tempSphere = Random.insideUnitSphere;
			spawnP = creator.getSpawnPosition();
			position = transform.TransformPoint(spawnP);
			if(initVelocity!=null)
				_startVelocity = initVelocity.GetVelocity(ref spawnP);
			if(initRotation!=null)
				_startRotation = initRotation.GetRotation();

			spawnParticle(
				particle,
				renderTransform,
				position,
				_startRotation,
				Random.Range(lifeSpanRange.x,lifeSpanRange.y),
				_startSize,
				_startVelocity);
			nextSpawnTime += spawnInterval;
		}
		_time = Time.time;
	}

	static void spawnParticle(nParticle particle,Transform world,Vector3 position,float rotation, float lifeSpan,float startSize,Vector3 starVelocity)
	{
		GameObject newParticle = GameObject.Instantiate(particle.gameObject,position,Quaternion.identity) as GameObject;
		newParticle.transform.parent = world;
		newParticle.transform.localScale = new Vector3(1f,1f,1f);
		nParticle nP = newParticle.GetComponent<nParticle>();
		nP.startSize = startSize;
		nP.startVelocity = starVelocity;
		nP.transform.localEulerAngles = new Vector3(0f,0f,rotation);
	}



}
