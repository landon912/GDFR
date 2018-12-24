using UnityEngine;

public class NewGameController : MonoBehaviour
{
    Animator anim = null;

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
        Toolbox.Instance.gameObject.RemoveFromDontDestroyOnLoad();
        GDFRNetworkManager.Instance.ShutdownAndLoadScene("MainMenu");
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
            GDFRNetworkManager.Instance.ChangeSceneOnAllClients("MainGame");
        }
    }
}