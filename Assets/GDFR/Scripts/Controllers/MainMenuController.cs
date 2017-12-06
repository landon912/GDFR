using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject MenuAudioController;

    public void Awake()
    {
        if (GameObject.FindGameObjectWithTag("MenuAudioController") == null)
        {
            Instantiate(MenuAudioController);
        }
    }

	public void NewGame()
	{
		SceneManager.LoadScene("NewGame");
	}

	public void ResumeGame()
	{
		Debug.Log("Resume Game");
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
