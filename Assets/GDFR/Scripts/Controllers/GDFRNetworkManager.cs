using System;
using System.Collections;
using System.Collections.Generic;
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
    public const short ClientCommand = 56;
    public const short ServerSendLobbyData = 58;

    public const short LobbyNumConnectionsChanged = 60;
    public const short LobbyClientConnected = 61;
    public const short SetupPlayerCountChanged = 62;
    public const short SetupDifficultyChanged = 63;
    public const short SetupCardVariantChanged = 64;
    public const short SetupRulesVariantChanged = 65;
    public const short SetupHumanToggleChanged = 66;
    public const short SetupAvatarChanged = 67;
    public const short SetupNameChanged = 68;
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
            return mInstance;
        }
    }

    public int LocalConnectionId
    {
        get { return localClient.connection.connectionId; }
    }

    private bool mSelfDestructOnSceneLoad = false;

    public int serverId = -1;
    public NetworkClient localClient;

    private int mNumPlayers = 0;
    public int NumPlayers
    {
        get { return mNumPlayers; }
        set
        {
            mNumPlayers = value;
            TriggerEventIfHost(MsgIndexes.LobbyNumConnectionsChanged, new IntegerMessage(value));
        }
    }

    public List<NetworkProfile> networkProfiles = new List<NetworkProfile>();

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
        localClient.RegisterHandler(MsgType.Connect, NetOnConnected);
        localClient.RegisterHandler(MsgIndexes.ServerRequestSceneChange, NetOnServerRequestSceneChange);
        localClient.RegisterHandler(MsgIndexes.ServerLeaving, NetOnServerFlagForDestruction);
        localClient.RegisterHandler(MsgIndexes.ServerFlagForDestruction, NetOnServerFlagForDestruction);
        localClient.RegisterHandler(MsgIndexes.ServerSendLobbyData, NetOnGetLobbyData);
    }

    public void SetupServerMessageHandlers()
    {
        NetworkServer.RegisterHandler(MsgType.Ready, NetOnClientReady);
        NetworkServer.RegisterHandler(MsgIndexes.LobbyClientConnected, NetOnClientConnect);
        NetworkServer.RegisterHandler(MsgType.Disconnect, NetOnClientDisconnect);
        NetworkServer.RegisterHandler(MsgIndexes.ClientCompletedSceneChange, NetOnClientCompletedSceneChange);
        NetworkServer.RegisterHandler(MsgIndexes.ClientRequestToLeave, NetOnClientRequestToLeave);
        NetworkServer.RegisterHandler(MsgIndexes.ClientCommand, NetOnClientCommand);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        localClient?.UnregisterHandler(MsgType.Connect);
        localClient?.UnregisterHandler(MsgIndexes.ServerRequestSceneChange);
        localClient?.UnregisterHandler(MsgIndexes.ServerLeaving);
        localClient?.UnregisterHandler(MsgIndexes.ServerFlagForDestruction);
        localClient?.UnregisterHandler(MsgIndexes.ServerSendLobbyData);

        NetworkServer.UnregisterHandler(MsgType.Ready);
        NetworkServer.UnregisterHandler(MsgIndexes.LobbyClientConnected);
        NetworkServer.UnregisterHandler(MsgType.Disconnect);
        NetworkServer.UnregisterHandler(MsgIndexes.ClientCompletedSceneChange);
        NetworkServer.UnregisterHandler(MsgIndexes.ClientRequestToLeave);
        NetworkServer.UnregisterHandler(MsgIndexes.ClientCommand);
    }

    public bool TriggerEventIfHost(short msgId, MessageBase message)
    {
        if (NetworkServer.active)
        {
            if (IsLocalClientTheHost())
            {
                NetworkServer.SendToAll(msgId, message);
                return true;
            }
        }

        return false;
    }

    public bool TriggerEventIfClient(short msgId, MessageBase message)
    {
        if (NetworkClient.active)
        {
            ClientCommandMessage mess = new ClientCommandMessage()
            {
                commandId = msgId,
                message = NetworkMessageHelper.NetworkMessageToByteArray(message),
            };

            localClient.Send(MsgIndexes.ClientCommand, mess);
            return true;
        }

        return false;
    }  

    public bool TriggerEvent(short msgId, MessageBase message)
    {
        if (TriggerEventIfHost(msgId, message))
            return true;
        return TriggerEventIfClient(msgId, message);
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
    
    /////////Client Callbacks///////////////////////////////////////////////////////////////////////////

    private void NetOnConnected(NetworkMessage message)
    {
        Debug.Log("Connected to server with message type " + MsgType.MsgTypeToString(message.msgType));

        PlayerConnectInfoMessage outgoingMess = new PlayerConnectInfoMessage
        {
            clientId = localClient.connection.connectionId,
            profile = new NetworkProfile(localClient.connection.connectionId, "NetworkPlayer " + localClient.connection.connectionId)
        };

        localClient.Send(MsgIndexes.LobbyClientConnected, outgoingMess);
    }

    private void NetOnServerRequestSceneChange(NetworkMessage message)
    {
        StartCoroutine(ChangeSceneAsync(message.ReadMessage<StringMessage>().value));
    }

    private void NetOnGetLobbyData(NetworkMessage message)
    {
        if (!IsLocalClientTheHost())
        {
            NumPlayers = 0;
            networkProfiles.Clear();

            LobbyDataMessage mess = message.ReadMessage<LobbyDataMessage>();

            foreach (NetworkProfile profile in mess.players)
            {
                NumPlayers++;
                networkProfiles.Add(profile);
            }
        }
    }
    
    private void NetOnServerFlagForDestruction(NetworkMessage message)
    {
        mSelfDestructOnSceneLoad = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSelfDestruct();
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////

    //////////Server Callbacks////////////////////////////////////////////////////////////////////////

    private void NetOnClientCommand(NetworkMessage message)
    {
        ClientCommandMessage mess = message.ReadMessage<ClientCommandMessage>();

        if(mess.commandId == MsgIndexes.SetupAvatarChanged)
        {
            MessageBase commandMessage = NetworkMessageHelper.BytesToNetworkMessage<PlayerAvatarMessage>(mess.message);

            Debug.Log("Replicating command to all clients");

            NetworkServer.SendToAll(mess.commandId, commandMessage);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void NetOnClientConnect(NetworkMessage message)
    {
        NumPlayers++;

        PlayerConnectInfoMessage mess = message.ReadMessage<PlayerConnectInfoMessage>();

        networkProfiles.Add(mess.profile);

        LobbyDataMessage outgoing = new LobbyDataMessage
        {
            players = networkProfiles.ToArray(),
        };

        NetworkServer.SendToAll(MsgIndexes.ServerSendLobbyData, outgoing);

        Debug.Log(mess.profile.networkName + " has successfully connected with connection Id " + mess.clientId);
    }

    private void NetOnClientDisconnect(NetworkMessage message)
    {
        NumPlayers--;
        Debug.Log("A client has successfully left a game");
    }

    private void NetOnClientReady(NetworkMessage message)
    {
        Debug.Log("A client has signaled that it is now ready");
    }

    private void NetOnClientCompletedSceneChange(NetworkMessage message)
    {
        ClientInfoMessage mess = message.ReadMessage<ClientInfoMessage>();
        Debug.Log("Client with connectionId " + mess.clientId + " has successfully loaded the scene and is now ready.");
    }

    private void NetOnClientRequestToLeave(NetworkMessage message)
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