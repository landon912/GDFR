using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NewGameController : NetworkBehaviour {

    Animator anim = null;

    public GameObject toolboxPrefab;
    public GameObject MenuAudioController;

    public override void OnStartServer()
    {
        //create toolbox
        SpawnToolbox();
        base.OnStartServer();
    }

    public void Start()
    {
        if (GameObject.FindGameObjectWithTag("MenuAudioController") == null)
        {
            Instantiate(MenuAudioController);
        }

        anim = GetComponent<Animator>();
    }

    [Server]
    private void SpawnToolbox()
    {
        GameObject obj = Instantiate(toolboxPrefab);
        NetworkServer.Spawn(obj);
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