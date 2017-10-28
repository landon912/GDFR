using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour {

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
		Debug.Log("Exit");
	}

}
