using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public TweenAlpha mainAlphaTweener;
    public UILabel playerCountLabel;

    public void StartServer()
    {
        GDFRNetworkManager.Instance.SetupHost();
    }

    public void JoinGame()
    {
        GDFRNetworkManager.Instance.SetupClient();
    }

    public void NewGame()
    {
        if (GDFRNetworkManager.Instance.IsClientTheHost(GDFRNetworkManager.Instance.localClient))
        {
            GDFRNetworkManager.Instance.ChangeSceneOnAllClients("NewGame");
        }
    }

    //public void BackToMainMenu()
    //{
    //    if (isServer)
    //    {
    //        NetworkManager.singleton.ServerChangeScene("MainMenu");
    //        NetworkServer.DisconnectAll();
    //        NetworkManager.singleton.StopHost();
    //    }
    //    else
    //    {
    //        NetworkManager.singleton.StopClient();
    //        SceneManager.LoadScene("MainMenu");
    //    }
    //}

    //void Update()
    //{
    //    playerCountLabel.text = "# of other Players: " + Network.connections.Length;

    //    if (isServer)
    //    {
    //        if (Network.connections.Length == 0 && NetworkServer.localConnections.Count == 0)
    //        {
    //            Debug.Log("There is no networking in this game, but we are still running a server to simulate");
    //        }
    //        else
    //        {
    //            Debug.Log("We have " + (Network.connections.Length + NetworkServer.localConnections.Count) + " connections (including ourselves)!");
    //        }
    //    }
    //    else
    //    {
    //        if (NetworkManager.singleton.client.isConnected)
    //        {
    //            Debug.Log("We are connected to " + Network.connections.Length + " peers!");
    //        }
    //    }

    //}



}