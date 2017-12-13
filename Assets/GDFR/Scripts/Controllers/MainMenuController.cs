using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject menuAudioController;
    public TweenAlpha mainAlphaTweener;

    public void Awake()
    {
        if (GameObject.FindGameObjectWithTag("MenuAudioController") == null)
        {
            Instantiate(menuAudioController);
        }
    }

	public void NewGame()
	{
		SceneManager.LoadScene("NewGame");
	}

	public void HowToPlay()
	{
	    SceneManager.LoadSceneAsync("Help_Additive", LoadSceneMode.Additive);
	}

	public void Settings()
	{
		Debug.Log("Settings");
	}

	public void Achievements()
	{
		Debug.Log("Achievements");
	}

	public void Credits()
	{
		Debug.Log("Credits");
	}

	public void Exit()
	{
	    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer ||
	        Application.platform == RuntimePlatform.LinuxPlayer)
	    {
	        Application.Quit();
	    }
	}
}
