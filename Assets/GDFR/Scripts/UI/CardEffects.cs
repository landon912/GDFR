using UnityEngine;

public class CardEffects : MonoBehaviour
{

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