using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyController : NetworkBehaviour
{
    public TweenAlpha mainAlphaTweener;
    public UILabel playerCountLabel;

    public void StartServer()
    {
        NetworkManager.singleton.StartHost();

        Debug.Log(Network.player.externalIP);
        Debug.Log(Network.player.externalPort);
        Debug.Log(Network.player.ipAddress);
        Debug.Log(Network.player.port);
    }

    public void NewGame()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("NewGame");
        }
    }

    public void JoinGame()
    {
        NetworkManager.singleton.StartClient();
        //NetworkConnectionError error = Network.Connect("localhost", 4444);
        //Debug.Log(error);
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

    void Update()
    {
        playerCountLabel.text = "# of other Players: " + Network.connections.Length;

        if (isServer)
        {
            if (Network.connections.Length == 0 && NetworkServer.localConnections.Count == 0)
            {
                Debug.Log("There is no networking in this game, but we are still running a server to simulate");
            }
            else
            {
                Debug.Log("We have " + (Network.connections.Length + NetworkServer.localConnections.Count) + " connections (including ourselves)!");
            }
        }
        else
        {
            if (NetworkManager.singleton.client.isConnected)
            {
                Debug.Log("We are connected to " + Network.connections.Length + " peers!");
            }
        }
        
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");
    }

}