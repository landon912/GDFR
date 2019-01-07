using UnityEngine;

namespace GDFR
{
    public class NewGameController : MonoBehaviour
    {
        public GameObject MenuAudioController;

        public void Start()
        {
            if (GameObject.FindGameObjectWithTag("MenuAudioController") == null)
            {
                Instantiate(MenuAudioController);
            }
        }

        public void BackScene()
        {
            Debug.Log("back button");
            Toolbox.Instance.gameObject.RemoveFromDontDestroyOnLoad();
            GDFRNetworkManager.Instance.ShutdownAndLoadScene("MainMenu");
        }

        public void StartGame()
        {
            Debug.Log("startGame button");

            if (GDFRNetworkManager.Instance.IsClientTheHost(GDFRNetworkManager.Instance.localClient))
            {
                FindObjectOfType<GameSettingUIEvents>().SelectRealAIProfiles();
                GDFRNetworkManager.Instance.ChangeSceneOnAllClients("MainGame");
            }
        }
    }
}