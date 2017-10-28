using UnityEngine;
using System.Collections;

public class playStars : MonoBehaviour {
	
	UITweener[] starTweeners;

	// Use this for initialization
	void Awake () {
		starTweeners = GetComponentsInChildren<UITweener>();
	}

	void OnEnable()
	{
		for(int t = 0;t<starTweeners.Length;t++)
		{
			starTweeners[t].delay = Random.Range(0f,1f);
			starTweeners[t].ResetToBeginning();
			starTweeners[t].enabled = true;
		}
		enabled = false;
	}


}
