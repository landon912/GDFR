using UnityEngine;

public class PlayTweens : MonoBehaviour
{
	public GameObject _target = null;
	public int _direction =  1;
	public int _group = 0;
	public bool _resetOnStart = true;
	public float randomMinDelay = 0f;
	public float randomMaxDelay = 0f;

	void Awake()
	{
		if(_target==null)
			_target = gameObject;
	}

	public void OnEnable()
	{
		PlayTweenGroup(_target,_group,_resetOnStart,_direction,randomMinDelay,randomMaxDelay);
		enabled = false;
	}

	public static void PlayTweenGroup(GameObject target,int group,bool resetOnStart,int direction,float minDelay = 0f,float maxDelay = 0f)
	{
		UITweener[] tList = target.GetComponentsInChildren<UITweener>();
		foreach(UITweener t in tList)
		{

			if(t.tweenGroup==group)
			{
				if(resetOnStart)
					t.ResetToBeginning();
				if(direction==1)
				{
					t.PlayForward();
				}
				if(direction==-1)
					t.PlayReverse();
			}
		}
	}
}