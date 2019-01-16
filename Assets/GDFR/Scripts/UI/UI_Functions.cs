using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Functions : MonoBehaviour
{
    public TextMeshProUGUI messageLabel;
    public GameObject gameEndButtons;
    public GameObject[] activateGameObject;

    public bool IsActive { private set; get; }

    void Start()
    {
        SetActivateGameObjectState(false);
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
        SetActivateGameObjectState(true);

        LeanTween.scale(messageLabel.GetComponent<RectTransform>(), Vector3.one, duration / 4.0f);

        yield return new WaitForSeconds(duration);

        LeanTween.scale(messageLabel.GetComponent<RectTransform>(), Vector3.zero, duration / 4.0f);

        yield return new WaitForSeconds(duration/4.0f);

        SetActivateGameObjectState(false);
    }

    public IEnumerator SendGameOverMessage(string message, bool enableButtons)
    {
        yield return new WaitForSeconds(0.01f);

        messageLabel.text = message;

        SetActivateGameObjectState(true, enableButtons);

        const float scaleTime = 1f;

        LeanTween.scale(messageLabel.GetComponent<RectTransform>(), Vector3.one, scaleTime);

        //PlayTweens.PlayTweenGroup(messageLabel.gameObject, 1, true, 1);
        if (enableButtons)
        {
            LeanTween.scale(gameEndButtons.transform.GetChild(0).GetComponent<RectTransform>(), Vector3.one, scaleTime);
            LeanTween.scale(gameEndButtons.transform.GetChild(1).GetComponent<RectTransform>(), Vector3.one, scaleTime);

            //PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(0).gameObject, 1, true, 1);
            //PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(1).gameObject, 1, true, 1);
        }
    }

    public IEnumerator HideGameOverMessage()
    {
        yield return new WaitForSeconds(0.01f);

        const float hideTime = 0.5f;

        LeanTween.scale(messageLabel.GetComponent<RectTransform>(), Vector3.zero, hideTime);
        LeanTween.scale(gameEndButtons.transform.GetChild(0).GetComponent<RectTransform>(), Vector3.zero, hideTime);
        LeanTween.scale(gameEndButtons.transform.GetChild(1).GetComponent<RectTransform>(), Vector3.zero, hideTime);

        //PlayTweens.PlayTweenGroup(messageLabel.gameObject, 2, true, 1);
        //PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(0).gameObject, 2, true, 1);
        //PlayTweens.PlayTweenGroup(gameEndButtons.transform.GetChild(1).gameObject, 2, true, 1);

        yield return new WaitForSeconds(hideTime);

        SetActivateGameObjectState(false);

        yield return new WaitForSeconds(0.2f);
    }
}