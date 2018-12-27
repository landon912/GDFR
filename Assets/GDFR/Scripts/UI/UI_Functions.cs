using UnityEngine;
using System.Collections;

public class UI_Functions : MonoBehaviour {

    public UILabel messageLabel;
    public GameObject gameEndButtons;
    public GameObject[] activateGameObject;

    public bool IsActive { private set; get; }

    private UILabel messageShadowLabel;

    void Start()
    {
        SetActivateGameObjectState(false);
        messageShadowLabel = messageLabel.transform.GetChild(0).GetComponent<UILabel>();
    }

    private void SetActivateGameObjectState(bool active, bool gameEnd = false)
    {
        IsActive = active;

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
	    messageShadowLabel.text = messageString;
	    SetActivateGameObjectState(true);

		PlayTweens.PlayTweenGroup(messageLabel.gameObject,1,true,1);

        yield return new WaitForSeconds(duration);

        PlayTweens.PlayTweenGroup(messageLabel.gameObject,2,true,1);

        yield return new WaitForSeconds(0.5f);

	    SetActivateGameObjectState(false);
	}

    public IEnumerator SendGameOverMessage(string message, bool enableButtons)
    {
        yield return new WaitForSeconds(0.01f);

        messageLabel.text = message;
        messageShadowLabel.text = message;

        SetActivateGameObjectState(true, true);

        PlayTweens.PlayTweenGroup(messageLabel.gameObject, 1, true, 1);
        if(enableButtons)
        {
            PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(0).gameObject, 1, true, 1);
            PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(1).gameObject, 1, true, 1);
        }
    }

    public IEnumerator HideGameOverMessage()
    {
        yield return new WaitForSeconds(0.01f);

        PlayTweens.PlayTweenGroup(messageLabel.gameObject, 2, true, 1);
        PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(0).gameObject, 2, true, 1);
        PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(1).gameObject, 2, true, 1);

        yield return new WaitForSeconds(0.5f);

        SetActivateGameObjectState(false);

        yield return new WaitForSeconds(0.2f);
    }
}