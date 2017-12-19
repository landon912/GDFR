using UnityEngine;
using System.Collections;

public class UI_Functions : MonoBehaviour {

	public UILabel messageLabel;
    public GameObject gameEndButtons;
	public GameObject[] activateGameObject;

    void Start()
    {
        SetActivateGameObjectState(false);
    }

    private void SetActivateGameObjectState(bool active, bool gameEnd = false)
    {
        messageLabel.gameObject.SetActive(active);

        gameEndButtons.SetActive(gameEnd);

        foreach (GameObject effect in activateGameObject)
        {
            effect.SetActive(active);
        }
    }

	public IEnumerator SendGameMessage(string messageString, float duration)
	{
		yield return new WaitForSeconds(0.01f);

	    messageLabel.text = messageString;
	    SetActivateGameObjectState(true);

		PlayTweens.PlayTweenGroup(messageLabel.gameObject,1,true,1);

        yield return new WaitForSeconds(duration);

        PlayTweens.PlayTweenGroup(messageLabel.gameObject,2,true,1);

        yield return new WaitForSeconds(0.5f);

	    SetActivateGameObjectState(false);
	}

    public IEnumerator SendGameOverMessage(string message)
    {
        yield return new WaitForSeconds(0.01f);

        messageLabel.text = message;

        SetActivateGameObjectState(true, true);

        PlayTweens.PlayTweenGroup(messageLabel.gameObject, 1, true, 1);
    }

    public IEnumerator HideGameOverMessage()
    {
        yield return new WaitForSeconds(0.01f);

        PlayTweens.PlayTweenGroup(messageLabel.gameObject, 2, true, 1);

        yield return new WaitForSeconds(0.5f);

        SetActivateGameObjectState(false);
    }
}