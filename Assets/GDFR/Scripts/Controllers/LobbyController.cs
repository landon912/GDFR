using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyController : NetworkBehaviour
{
    public TweenAlpha mainAlphaTweener;

    public void NewGame()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("NewGame");
        }
    }

    public void BackToMainMenu()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("MainMenu");
            NetworkServer.DisconnectAll();
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene("MainMenu");
        }
    }
}