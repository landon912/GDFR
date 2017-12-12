﻿using UnityEngine;
using System.Collections;

public class UI_Functions : MonoBehaviour {

	public UILabel messageLabel;
	public GameObject[] activateGameObject;

	public IEnumerator SendGameMessage(string messageString, float duration)
	{
		yield return new WaitForSeconds(0.01f);
        
	    foreach (GameObject effect in activateGameObject)
	    {
	        effect.SetActive(true);
	    }

        messageLabel.text = messageString;
		messageLabel.enabled = true;
		PlayTweens.PlayTweenGroup(messageLabel.gameObject,1,true,1);

        yield return new WaitForSeconds(duration);

        PlayTweens.PlayTweenGroup(messageLabel.gameObject,2,true,1);

        yield return new WaitForSeconds(0.5f);

	    foreach (GameObject effect in activateGameObject)
	    {
			effect.SetActive(false);
        }
        messageLabel.enabled = false;
	}
}
