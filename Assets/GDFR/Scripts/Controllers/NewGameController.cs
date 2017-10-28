using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewGameController : MonoBehaviour {

    Animator anim = null;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void backScene()
    {
        Debug.Log("back button");
        SceneManager.LoadScene("MainMenu");
    }
    
    public void animateButon()
    {
        if (null != anim)
        {
            Debug.Log("Playing anim");
            anim.Play("Pressed");
        }
    }

    public void startGame()
    {
        Debug.Log("startGame button");
        SceneManager.LoadScene("MainGame");
    }
}
