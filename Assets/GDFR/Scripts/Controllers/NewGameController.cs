using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NewGameController : MonoBehaviour
{

    Animator anim = null;

    public GameObject toolboxPrefab;
    public GameObject MenuAudioController;

    public void Start()
    {
        if (GameObject.FindGameObjectWithTag("MenuAudioController") == null)
        {
            Instantiate(MenuAudioController);
        }

        anim = GetComponent<Animator>();
    }

    public void backScene()
    {
        Debug.Log("back button");
        if (GDFRNetworkManager.Instance.IsClientTheHost(GDFRNetworkManager.Instance.localClient))
        {
            GDFRNetworkManager.Instance.ChangeSceneOnAllClients("Lobby");
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

        if (GDFRNetworkManager.Instance.IsClientTheHost(GDFRNetworkManager.Instance.localClient))
        {
            FindObjectOfType<GameSettingUIEvents>().SelectRealAIProfiles();
            Destroy(AudioController.Instance.gameObject);
            GDFRNetworkManager.Instance.ChangeSceneOnAllClients("MainGame");
        }
    }
}