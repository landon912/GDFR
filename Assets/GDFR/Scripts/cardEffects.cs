using UnityEngine;
using System.Collections;

public class cardEffects : MonoBehaviour {

	AnimationClip shakeClip = null;
	Animation anim = null;

	void Awake()
	{
		anim = GetComponent<Animation>();
	}

	//play the shake animation
	public void Card_Shake()
	{
		anim.Play("Shake01");
	}

}
