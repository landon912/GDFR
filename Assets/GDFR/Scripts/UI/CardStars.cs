using UnityEngine;

public class CardStars : MonoBehaviour
{
    private GameObject[] children;

	// Use this for initialization
	void Awake ()
    {
        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }
	}

    public void Show()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(true);
        }
    }

    public void Play()
	{
	    UITweener[] starTweeners = GetComponentsInChildren<UITweener>();

        foreach (UITweener tweener in starTweeners)
        {
            tweener.delay = Random.Range(0f,1f);
            tweener.ResetToBeginning();
            tweener.enabled = true;
        }
	}

    public void Hide()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(false);
        }
    }
}