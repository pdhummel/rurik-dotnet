using LiteNetLib;
using LiteNetLib.Utils;
using System.Reflection;
using System.Text.Json;
using static rurik.GameEvent;
using rurik.Actions;

namespace rurik;

public class Server
{
    private NetManager? server;

    public Dictionary<NetPeer, string> PeerToPlayerName { get; set; } = new Dictionary<NetPeer, string>();
    public Dictionary<string, NetPeer> PlayerNameToPeer { get; set; } = new Dictionary<string, NetPeer>();

    private EventBasedNetListener? listener;
    private Thread? serverThread;
    private bool isRunning = false;
    private string? key;
    private int maxPeers;
    public ServerGameState? GameState { get; set; }
    private bool initialSync = false;
    public Games Games { get; set; } = new Games();
    Random random = new Random();
    int lastQueueSize = 0;

    public void StartAsHost(GameSettings gameSettings, string key)
    {
        Globals.Log("StartAsHost(): enter");
        this.maxPeers = 8; // gameSettings.NumberOfHumans;
        this.key = key;
        GameState = new ServerGameState(gameSettings);
        listener = new EventBasedNetListener();

        // Set up event handlers for connection/data
        listener.ConnectionRequestEvent += OnConnectionRequest;
        listener.PeerConnectedEvent += OnPeerConnected;
        listener.NetworkReceiveEvent += OnNetworkReceive;
        listener.PeerDisconnectedEvent += OnPeerDisconnected;

        server = new NetManager(listener)
        {
            UnsyncedEvents = true
        };

        // Start the server manager
        server.Start(gameSettings.Port);
        isRunning = true;

        // Create and start the new thread for the server's polling loop
        serverThread = new Thread(new ThreadStart(ServerLoop))
        {
            IsBackground = true // Ensures thread closes with the main app
        };
        serverThread.Start();
    }

    public void RestoreHost(GameSettings gameSettings, string key)
    {
        Globals.Log("RestoreHost(): enter");
        this.maxPeers = 8; //gameSettings.NumberOfHumans;
        this.key = key;
        listener = new EventBasedNetListener();

        // Set up event handlers for connection/data
        listener.ConnectionRequestEvent += OnConnectionRequest;
        listener.PeerConnectedEvent += OnPeerConnected;
        listener.NetworkReceiveEvent += OnNetworkReceive;
        listener.PeerDisconnectedEvent += OnPeerDisconnected;

        server = new NetManager(listener)
        {
            UnsyncedEvents = true
        };

        // Start the server manager
        server.Start(gameSettings.Port);
        isRunning = true;

        // Create and start the new thread for the server's polling loop
        serverThread = new Thread(new ThreadStart(ServerLoop))
        {
            IsBackground = true // Ensures thread closes with the main app
        };
        serverThread.Start();
    }


    private void ServerLoop()
    {
        //GameLogic = new GameLogic(this);
        //GameLogic.StartGame();

        int sleepTime = 1000;
        Globals.Log("ServerLoop(): Server polling");
        // This is the server's polling loop, which runs continuously on its own thread.
        while (isRunning)
        {
            server?.PollEvents();
            // TODO
            //if (!initialSync && ServerGameState.PlayerJoined.Count >= gameState.GameSettings.NumberOfHumans)
            //{
            //    Globals.Log("ServerLoop(): all clients joined");
            //    GameLogic.gameStarted(this);
            //    sendGamePlayEvent(new GameEvent(GAME_EVENT_JOINED_GAME));
            //}

            Thread.Sleep(sleepTime); // Adjust sleep time to control CPU usage.
        }
    }

    public void sendGameState()
    {
        if (server != null)
        {
            int count = server.ConnectedPeerList.Count;
            for (int i = 0; i < count; i++)
            {
                if (i <= server.ConnectedPeerList.Count)
                {
                    try
                    {
                        NetPeer peer = server.ConnectedPeerList[i];
                        sendGameState(peer);
                    }
                    catch (Exception ex)
                    {
                        Globals.Log("sendGameState(): Exception:" + ex +
                        ", Count=" + server.ConnectedPeerList.Count + ", i=" + i);
                    }
                }
                else
                {
                    Globals.Log("sendGameState(): Count=" + server.ConnectedPeerList.Count + ", i=" + i);
                }

            }
        }
    }


    public void sendGameState(NetPeer peer)
    {
        GameEvent gameEvent = new GameEvent(EVENT_TYPE_GAME_STATE_UPDATE);
        gameEvent.GameState = GameState;
        string jsonString = JsonSerializer.Serialize(gameEvent);
        sendJsonString(peer, jsonString);
    }

    public void sendGamePlayEvent(GameEvent gameEvent)
    {
        if (server != null)
        {
            int count = server.ConnectedPeerList.Count;
            for (int i = 0; i < count; i++)
            {
                if (i < server.ConnectedPeerList.Count)
                {
                    try
                    {
                        NetPeer peer = server.ConnectedPeerList[i];
                        sendGamePlayEvent(peer, gameEvent);
                    }
                    catch (Exception ex)
                    {
                        Globals.Log("sendGamePlayEvent(): Exception:" + ex +
                        ", Count=" + server.ConnectedPeerList.Count + ", i=" + i);
                    }
                }
                else
                {
                    Globals.Log("sendGamePlayEvent(): Count=" + server.ConnectedPeerList.Count + ", i=" + i);
                }
            }
        }

    }


    public void sendGamePlayEvent(string color, GameEvent gameEvent)
    {
        if (server != null)
        {
            // TODO
        }
    }

    public void sendGamePlayEvent(NetPeer peer, GameEvent gameEvent)
    {
        string jsonString = JsonSerializer.Serialize(gameEvent);
        sendJsonString(peer, jsonString);
    }

    public void sendJsonString(NetPeer peer, String jsonString)
    {
        int value = random.Next(0, 60);
        if (value == 0)
            checkQueueCount(peer);
        NetDataWriter writer = new NetDataWriter();
        if (server != null)
        {
            writer.Put(jsonString);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            writer.Reset();
        }
    }

    private void checkQueueCount()
    {
        if (server.ConnectedPeerList.Count > 0)
        {
            int peerIndex = random.Next(0, server.ConnectedPeerList.Count);
            NetPeer peer = server.ConnectedPeerList[peerIndex];
            checkQueueCount(peer);
        }
    }

    private void checkQueueCount(NetPeer peer)
    {
        byte channelId = 0;
        bool isOrdered = true;
        int queueCount = peer.GetPacketsCountInReliableQueue(channelId, isOrdered);
        Globals.Log("sendJsonString(): Packets in queue: " + queueCount);
        // Throttle server processing until the network message queue is caught up a little.
        while (queueCount > 50000)
        {
            Thread.Sleep(1000);
            queueCount = peer.GetPacketsCountInReliableQueue(channelId, isOrdered);
        }
        Globals.Log("sendJsonString(): Packets in queue: " + queueCount);
    }

    private void StopServer()
    {
        isRunning = false;
        if (serverThread != null && serverThread.IsAlive)
        {
            serverThread.Join(); // Wait for the server thread to finish gracefully
        }
        server?.Stop();
    }

    private void Update()
    {
        server?.PollEvents();
    }

    private void OnDestroy()
    {
        server?.Stop();
    }

    // --- LiteNetLib Event Handlers ---
    private void OnConnectionRequest(ConnectionRequest request)
    {
        Globals.Log($"OnConnectionRequest(): Incoming connection request to Server from: {request.RemoteEndPoint}, data=" + request.Data);
        // In a real application, you would add validation here.
        if (server?.ConnectedPeersCount < maxPeers)
        {
            request.AcceptIfKey(this.key);
            Globals.Log("OnConnectionRequest(): connection accepted by Server");
        }
        else
        {
            request.Reject();
            Globals.Log("OnConnectionRequest(): connection rejected by Server b/c limit to connected peers. (player count)");
        }
    }

    private void OnPeerConnected(NetPeer peer)
    {
        Globals.Log($"OnPeerConnected(): Peer connected to Server: {peer.Address}");
    }

    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var jsonString = reader.GetString();
        Globals.Log("Server.OnNetworkReceive(): " + jsonString);
        reader.Recycle(); // Free up the data reader
        PlayerAction? action =
                JsonSerializer.Deserialize<PlayerAction>(jsonString);
        PlayerAction subClassAction = action.makeSubclass();
        subClassAction.MessageAsJson = jsonString;
        MethodInfo executeMethod = subClassAction.GetType().GetMethod("deserializeAndExecute");
        object[] parameters = new object[] { peer, this };

        executeMethod?.Invoke(subClassAction, parameters);

    }

    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Globals.Log($"OnPeerDisconnected(): Peer disconnected: {peer.Address} from Server. Reason: {disconnectInfo.Reason}");
        if (PeerToPlayerName.ContainsKey(peer))
        {
            string playerName = PeerToPlayerName[peer];
            // TODO
        }
    }



}
