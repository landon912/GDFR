using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class MsgIndexes
{
    public const short ServerStarted = MsgType.Highest + 1;
    public const short SceneChangeRequested = MsgType.Highest + 2;
    public const short SceneChangeCompleted = MsgType.Highest + 3;
    public const short LobbyNumConnectionsChanged = MsgType.Highest + 4;
    public const short SetupPlayerCountChanged = MsgType.Highest + 5;
    public const short SetupDifficultyChanged = MsgType.Highest + 6;
    public const short SetupCardVariantChanged = MsgType.Highest + 7;
    public const short SetupRulesVariantChanged = MsgType.Highest + 8;
}

public class GDFRNetworkManager : MonoBehaviour
{
    private static GDFRNetworkManager mInstance;
    public static GDFRNetworkManager Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = FindObjectOfType<GDFRNetworkManager>();
                //mInstance = new GameObject("Network Manager").AddComponent<GDFRNetworkManager>();

            return mInstance;
        }
    }

    public int serverId = -1;
    public NetworkClient localClient;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool IsLocalClientTheHost()
    {
        return IsClientTheHost(localClient);
    }

    public bool IsClientTheHost(NetworkClient clientToCheck)
    {
        return clientToCheck.connection.connectionId == serverId;
    }

    public void SetupHost()
    {
        NetworkServer.Listen(4444);
        localClient = ClientScene.ConnectLocalServer();
        serverId = localClient.connection.connectionId;
        ClientScene.Ready(localClient.connection);
        SetupBaseMessageHandlers();
        SetupServerMessageHandlers();
    }

    public void SetupClient()
    {
        localClient = new NetworkClient();  
        localClient.Connect("127.0.0.1", 4444);
        SetupBaseMessageHandlers();
    }

    public void SetupBaseMessageHandlers()
    {
        localClient.RegisterHandler(MsgType.Connect, OnConnected);
        localClient.RegisterHandler(MsgIndexes.SceneChangeRequested, OnSceneChangeRequested);
    }

    public void SetupServerMessageHandlers()
    {
        NetworkServer.RegisterHandler(MsgType.Ready, OnClientReady);
        NetworkServer.RegisterHandler(MsgIndexes.SceneChangeCompleted, OnClientSceneChangeCompleted);
    }

    public void TriggerEventIfHost(short msgID, MessageBase message)
    {
        if (NetworkServer.active)
        {
            if (IsLocalClientTheHost())
            {
                NetworkServer.SendToAll(msgID, message);
            }
        }
    }

    public void ShutdownAndLoadScene(string newScene)
    {
        if (NetworkServer.active)
        {
            ChangeSceneOnAllClients(newScene);
            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
            Destroy
        }
        else if (NetworkClient.active)
        {
            localClient.Disconnect();
            localClient.Shutdown();
            SceneManager.LoadScene(newScene);
            Destroy(gameObject);
        }
        else
        {
            SceneManager.LoadScene(newScene);
            Destroy(gameObject);
        }
    }

    //public NetworkClient FindNetworkClientFromId(int connectionId)
    //{
    //    if (IsLocalClientTheHost(localClient))
    //    {
    //        for (int i = 0; i < NetworkClient.allClients.Count; i++)
    //        {
    //            if (NetworkClient.allClients[i].connection.connectionId == connectionId)
    //            {
    //                return NetworkClient.allClients[i];
    //            }
    //        }

    //        Debug.LogError("Client not found");
    //        return null;
    //    }

    //    Debug.LogError("This is a server only method!");
    //    return null;
    //}

    IEnumerator ChangeSceneAsync(string sceneName)
    {
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);

        yield return new WaitUntil(() => sceneLoadingOperation.isDone);


        if (!IsClientTheHost(localClient))
        {
            Debug.Log("sending my info to be set ready");
            ClientInfoMessage message = new ClientInfoMessage
            {
                clientId = localClient.connection.connectionId,
                message = SceneManager.GetActiveScene().name
            };
            ClientScene.Ready(localClient.connection);
            localClient.Send(MsgIndexes.SceneChangeCompleted, message);
        }
    }

    //////////Callbacks////////////////////////////////////////////////////////////////////////////////
    private void OnSceneChangeRequested(NetworkMessage message)
    {
        StartCoroutine(ChangeSceneAsync(message.ReadMessage<StringMessage>().value));
    }

    private void OnConnected(NetworkMessage message)
    {
        Debug.Log("Connected to server with message type " + MsgType.MsgTypeToString(message.msgType));
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////

    //////////Server Callbacks////////////////////////////////////////////////////////////////////////
    private void OnClientSceneChangeCompleted(NetworkMessage message)
    {
        ClientInfoMessage mess = message.ReadMessage<ClientInfoMessage>();
        Debug.Log("Client with connectionId " + mess.clientId + " has successfully loaded the scene and is now ready.");
    }

    private void OnClientReady(NetworkMessage message)
    {
        Debug.Log("A client has signaled that it is now ready");
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void ChangeSceneOnAllClients(string newSceneName)
    {
        Debug.Log("ServerChangeScene to " + newSceneName);

        NetworkServer.SetAllClientsNotReady();
        NetworkServer.SendToAll(MsgIndexes.SceneChangeRequested, new StringMessage(newSceneName));
    }
}