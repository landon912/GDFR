using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameController : MonoBehaviour {

    Animator anim = null;

    public GameObject MenuAudioController;

    public void Awake()
    {
        if (GameObject.FindGameObjectWithTag("MenuAudioController") == null)
        {
            Instantiate(MenuAudioController);
        }
    }

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
        Destroy(AudioController.Instance.gameObject);
        SceneManager.LoadScene("MainGame");
    }
}