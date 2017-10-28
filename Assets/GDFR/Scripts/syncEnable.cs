using UnityEngine;
using System.Collections;

public class syncEnable : MonoBehaviour {

	MonoBehaviour source = null;
	MonoBehaviour[] target;

	void OnEnable()
	{
		foreach(MonoBehaviour m in target)
			m.enabled = true;
	}

	void OnDisable()
	{
		foreach(MonoBehaviour m in target)
			m.enabled = false;
	}
}
