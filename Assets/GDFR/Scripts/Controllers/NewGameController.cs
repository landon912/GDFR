using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NewGameController : NetworkBehaviour {

    Animator anim = null;

    public GameObject MenuAudioController;

    public void Start()
    {
        Debug.Log(NetworkServer.active);

        if (Network.connections.Length == 0)
        {
            Debug.Log("There is no networking in this game, but we are still running a server to simulate");
        }
        else
        {
            Debug.Log("We are connected to " + Network.connections.Length + " peers!");
        }

        if (GameObject.FindGameObjectWithTag("MenuAudioController") == null)
        {
            Instantiate(MenuAudioController);
        }

        anim = GetComponent<Animator>();
    }

    public void backScene()
    {
        Debug.Log("back button");
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("Lobby");
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene("Lobby");
        }
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

        if (isServer)
        {
            FindObjectOfType<GameSettingUIEvents>().SelectRealAIProfiles();
            Destroy(AudioController.Instance.gameObject);
            NetworkManager.singleton.ServerChangeScene("MainGame");
        }
    }
}