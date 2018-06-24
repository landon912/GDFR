using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class MsgIndexs
{
    public const short MessagingReady = MsgType.Highest + 1;
    public const short SceneChangeRequested = MsgType.Highest + 2;
    public const short SceneChangeCompleted = MsgType.Highest + 3;
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

    public bool IsClientTheHost(NetworkClient clientToCheck)
    {
        return clientToCheck.connection.connectionId == serverId;
    }

    public void SetupHost()
    {
        NetworkServer.Listen(4444);
        localClient = ClientScene.ConnectLocalServer();
        serverId = localClient.connection.connectionId;
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
        localClient.RegisterHandler(MsgIndexs.SceneChangeRequested, OnSceneChangeRequested);
    }

    public void SetupServerMessageHandlers()
    {
        localClient.RegisterHandler(MsgType.Ready, OnClientReady);
        localClient.RegisterHandler(MsgIndexs.SceneChangeCompleted, OnClientSceneChangeCompleted);
    }

    public NetworkClient FindNetworkClientFromId(int connectionId)
    {
        if (IsClientTheHost(localClient))
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

    /*
    //private void SendReadyToBeginMessage(int myId)
    //{
    //    var msg = new IntegerMessage(myId);
    //    m_client.Send(MyBeginMsg, msg);
    //}

    //private void OnServerReadyToBeginMessage(NetworkMessage netMessage)
    //{
    //    IntegerMessage message = netMessage.ReadMessage<IntegerMessage>();
    //    Debug.Log("recieved OnServerReadyToBeginMessage " + message.value);
    //}
    */

    IEnumerator ChangeSceneAsync(string sceneName)
    {
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);

        //while (!sceneLoadingOperation.isDone)
        //{
        //    Debug.Log("yielding");
        //    yield return null;
        //}

        yield return new WaitUntil(() => sceneLoadingOperation.isDone);

        Debug.Log("sending my info to be set ready");
        ClientInfoMessage message = new ClientInfoMessage
        {
            clientId = localClient.connection.connectionId,
            message = SceneManager.GetActiveScene().name
        };
        localClient.Send(MsgIndexs.SceneChangeCompleted, message);
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
        Debug.Log("A client has successfully loaded the scene " + mess.message);

        NetworkClient loadedClient = FindNetworkClientFromId(mess.clientId);
        NetworkServer.SetClientReady(loadedClient.connection);

        Debug.Log("The server has set client #" + mess.clientId + " as ready");
    }

    private void OnClientReady(NetworkMessage message)
    {
        Debug.Log("A client is now ready with message type" + message.ReadMessage<StringMessage>().value);
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void ChangeSceneOnAllClients(string newSceneName)
    {
        Debug.Log("ServerChangeScene to " + newSceneName);

        NetworkServer.SetAllClientsNotReady();
        NetworkServer.SendToAll(MsgIndexs.SceneChangeRequested, new StringMessage(newSceneName));
    }
}