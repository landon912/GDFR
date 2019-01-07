using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyController : MonoBehaviour
{
    public TweenAlpha mainAlphaTweener;
    public UILabel playerCountLabel;

    public void StartServer()
    {
        GDFRNetworkManager.Instance.SetupHost();
        StartNetworkHandlers();
    }

    public void JoinGame()
    {
        GDFRNetworkManager.Instance.SetupClient();
        StartNetworkHandlers();
    }

    private void StartNetworkHandlers()
    {
        GDFRNetworkManager.Instance.localClient.RegisterHandler(MsgIndexes.LobbyNumConnectionsChanged, OnNumConnectionChanged);
    }

    private void OnDisable()
    {
        GDFRNetworkManager.Instance?.localClient?.UnregisterHandler(MsgIndexes.LobbyNumConnectionsChanged);
    }

    private void OnNumConnectionChanged(NetworkMessage message)
    {
        int count = message.ReadMessage<IntegerMessage>().value;

        //only update if not host
        if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            GDFRNetworkManager.Instance.NumPlayers = count;
        }

        playerCountLabel.text = "# of Players: " + count;
    }

    public void NewGame()
    {
        if (GDFRNetworkManager.Instance.IsClientTheHost(GDFRNetworkManager.Instance.localClient))
        {
            GDFRNetworkManager.Instance.ChangeSceneOnAllClients("NewGame");
        }
    }

    public void BackToMainMenu()
    {
        GDFRNetworkManager.Instance.ShutdownAndLoadScene("MainMenu");
    }
}