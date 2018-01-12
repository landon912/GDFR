using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScript : MonoBehaviour {

	public void loadTileScene()
	{
		SceneManager.LoadScene("MainMenu");
	}
}