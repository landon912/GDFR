using UnityEngine;
using UnityEngine.UI;

public class nParticle : MonoBehaviour {
	
	public Vector3 startVelocity = new Vector3(0f,0.01f,0f);
	public float startSize = 1f;
	public AnimationCurve sizeOverLife;
	Vector3 velocity;
	Image sprite;
	Vector3 position;
	nPhysics physics;
	nKiller killer;

	void Awake(){
		sprite = GetComponent<Image>();
		sprite.enabled = false;
		physics = GetComponent<nPhysics>();
		killer = GetComponent<nKiller>();
	}

	// Use this for initialization
	void Start () {
		position = transform.localPosition;
		velocity = startVelocity;
	}
	
	// Update is called once per frame
	void FixedUpdate(){
		sprite.enabled = true;

		//movement
		position += velocity;

		//physics
		physics.Apply(ref position);

		transform.localPosition=position;


		//size
		transform.localScale = startSize * (sizeOverLife.Evaluate(killer.age) * new Vector3(1f,1f,1f));

	}
}
