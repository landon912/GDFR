using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class MsgIndexes
{
    public const short ServerStarted = 50;
    public const short ServerRequestSceneChange = 51;
    public const short ClientCompletedSceneChange = 52;
    public const short ServerLeaving = 53;
    public const short ClientRequestToLeave = 54;
    public const short ServerFlagForDestruction = 55;

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

    private int mNumPlayers = 0;
    public int NumPlayers
    {
        get { return mNumPlayers; }
        private set
        {
            mNumPlayers = value;
            TriggerEventIfHost(MsgIndexes.LobbyNumConnectionsChanged, new IntegerMessage(value));
        }
    }

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

        NumPlayers++;
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
        localClient.RegisterHandler(MsgIndexes.ServerRequestSceneChange, OnServerRequestSceneChange);
        localClient.RegisterHandler(MsgIndexes.ServerLeaving, OnServerFlagForDestruction);
        localClient.RegisterHandler(MsgIndexes.ServerFlagForDestruction, OnServerFlagForDestruction);
    }

    public void SetupServerMessageHandlers()
    {
        NetworkServer.RegisterHandler(MsgType.Ready, OnClientReady);
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnect);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnect);
        NetworkServer.RegisterHandler(MsgIndexes.ClientCompletedSceneChange, OnClientCompletedSceneChange);
        NetworkServer.RegisterHandler(MsgIndexes.ClientRequestToLeave, OnClientRequestToLeave);
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

    public NetworkClient FindNetworkClientFromId(int connectionId)
    {
        if (IsLocalClientTheHost())
        {
            for (int i = 0; i < NetworkClient.allClients.Count; i++)
            {
                if (NetworkClient.allClients[i].connection.connectionId == connectionId)
                {
                    return NetworkClient.allClients[i];
                }
            }

            Debug.LogError("Client not found");
            return null;
        }

        Debug.LogError("This is a server only method!");
        return null;
    }

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
            localClient.Send(MsgIndexes.ClientCompletedSceneChange, message);
        }
    }

    public void ShutdownAndLoadScene(string newScene)
    {
        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(MsgIndexes.ServerLeaving, new EmptyMessage());
            ChangeSceneOnAllClients(newScene);
        }
        else if (NetworkClient.active)
        {
            localClient.Send(MsgIndexes.ClientRequestToLeave, new IntegerMessage(localClient.connection.connectionId));
        }
    }

    private void CheckSelfDestruct()
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
    
    /////////Client Callbacks////////////////////////////////////////////////////////////////////////////
    private void OnServerRequestSceneChange(NetworkMessage message)
    {
        StartCoroutine(ChangeSceneAsync(message.ReadMessage<StringMessage>().value));
    }

    private void OnConnected(NetworkMessage message)
    {
        Debug.Log("Connected to server with message type " + MsgType.MsgTypeToString(message.msgType));
    }

    private void OnServerFlagForDestruction(NetworkMessage message)
    {
        mSelfDestructOnSceneLoad = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSelfDestruct();
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////

    //////////Server Callbacks////////////////////////////////////////////////////////////////////////
    private void OnClientConnect(NetworkMessage message)
    {
        NumPlayers++;
        Debug.Log("A client has successfully connected");
    }

    private void OnClientDisconnect(NetworkMessage message)
    {
        NumPlayers--;
        Debug.Log("A client has successfully left a game");
    }

    private void OnClientReady(NetworkMessage message)
    {
        Debug.Log("A client has signaled that it is now ready");
    }

    private void OnClientCompletedSceneChange(NetworkMessage message)
    {
        ClientInfoMessage mess = message.ReadMessage<ClientInfoMessage>();
        Debug.Log("Client with connectionId " + mess.clientId + " has successfully loaded the scene and is now ready.");
    }

    private void OnClientRequestToLeave(NetworkMessage message)
    {
        int clientConnId = message.ReadMessage<IntegerMessage>().value;

        Debug.Log("A client has requested to leave. Preparing client id " + clientConnId + " for departure.");

        NetworkServer.SendToClient(clientConnId, MsgIndexes.ServerFlagForDestruction, new EmptyMessage());
        NetworkServer.SendToClient(clientConnId, MsgIndexes.ServerRequestSceneChange, new StringMessage("MainMenu"));
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void ChangeSceneOnAllClients(string newSceneName)
    {
        Debug.Log("ServerChangeScene to " + newSceneName);

        NetworkServer.SetAllClientsNotReady();
        NetworkServer.SendToAll(MsgIndexes.ServerRequestSceneChange, new StringMessage(newSceneName));
    }
}