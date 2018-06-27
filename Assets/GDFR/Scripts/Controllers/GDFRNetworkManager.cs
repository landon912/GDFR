using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class MsgIndexes
{
    public const short ServerStarted = 50;
    public const short SceneChangeRequested = 51;
    public const short SelfDestructSceneChangeRequested = 52;
    public const short SceneChangeCompleted = 53;

    public const short LobbyNumConnectionsChanged = 60;
    public const short SetupPlayerCountChanged = 61;
    public const short SetupDifficultyChanged = 62;
    public const short SetupCardVariantChanged = 63;
    public const short SetupRulesVariantChanged = 64;
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

    private bool mSelfDestructOnSceneLoad = false;

    public int serverId = -1;
    public NetworkClient localClient;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        localClient.RegisterHandler(MsgIndexes.SelfDestructSceneChangeRequested, OnSelfDestructSceneChangeRequested);
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
        SelfDestructChangesSceneOnAllClients(newScene);
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

    IEnumerator ChangeSceneAsync(string sceneName, bool selfDestructOnLoad = false)
    {
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);

        yield return new WaitUntil(() => sceneLoadingOperation.isDone);

        if (!IsClientTheHost(localClient) && !selfDestructOnLoad)
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

    private void ChangeScene(string sceneName, bool selfDestructOnLoad = false)
    {
        SceneManager.LoadScene(sceneName);

        if (!IsClientTheHost(localClient) && !selfDestructOnLoad)
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)

    {
        if (mSelfDestructOnSceneLoad)
        {
            if (NetworkServer.active)
            {
                NetworkServer.DisconnectAll();
                NetworkServer.Shutdown();

                localClient.Disconnect();
                localClient.Shutdown();
            }
            else if (NetworkClient.active)
            {
                localClient.Disconnect();
                localClient.Shutdown();
            }

            Destroy(gameObject);
        }
    }

    //////////Callbacks////////////////////////////////////////////////////////////////////////////////
    private void OnSceneChangeRequested(NetworkMessage message)
    {
        StartCoroutine(ChangeSceneAsync(message.ReadMessage<StringMessage>().value));
    }

    private void OnSelfDestructSceneChangeRequested(NetworkMessage message)
    {
        mSelfDestructOnSceneLoad = true;
        ChangeScene(message.ReadMessage<StringMessage>().value, true);
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

    public void SelfDestructChangesSceneOnAllClients(string newSceneName)
    {
        Debug.Log("BlindServerChangeScene to " + newSceneName);

        NetworkServer.SetAllClientsNotReady();
        NetworkServer.SendToAll(MsgIndexes.SelfDestructSceneChangeRequested, new StringMessage(newSceneName));
    }
}