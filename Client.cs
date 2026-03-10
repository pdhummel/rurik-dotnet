using LiteNetLib;
using LiteNetLib.Utils;
using System.Text.Json;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using rurik.Actions;

namespace rurik;

public class Client(RurikMonoGame game)
{
    private NetManager? netmanagerclient;
    private EventBasedNetListener? listener;
    private Thread? clientThread;
    private Thread? processGameEventQueueThread;
    public string? ClientIdentifier { get; set; }   // this is the player name
    private NetPeer? serverPeer;

    public RurikMonoGame RurikMonoGame { get; set; } = game;
    public JoinGameValues? JoinGameValues { get; set; }

    public ServerGameState? GameState { get; set; }
    public Games? Games {get;set;}
    ConcurrentQueue<GameEvent> gameEventExecutionQueue = new ConcurrentQueue<GameEvent>();

    public void Connect(JoinServerValues joinServerValues, string key)
    {
        ClientIdentifier = joinServerValues.Name;
        listener = new EventBasedNetListener();
        listener.PeerConnectedEvent += OnPeerConnected;
        listener.NetworkReceiveEvent += OnNetworkReceive;
        listener.PeerDisconnectedEvent += OnPeerDisconnected;

        netmanagerclient = new NetManager(listener)
        {
            UnconnectedMessagesEnabled = true,
            UnsyncedEvents = true
        };
        netmanagerclient.Start();
        string host = joinServerValues.HostIp;
        int port = joinServerValues.Port;
        serverPeer = netmanagerclient.Connect(host, port, key); // Use the same key as the server
        Globals.Log($"Connect(): Client attempting to connect to {host}:{port}");
        // Create and start the new thread for the client's polling loop
        clientThread = new Thread(new ThreadStart(ClientLoop))
        {
            IsBackground = true // Ensures thread closes with the main app
        };
        clientThread.Start();

        // TODO: tell the server we are ready.
        //PlayerAction action = new(serverPeer, ClientIdentifier, "connect");
    }


    private void OnDestroy()
    {
        netmanagerclient?.Stop();
    }

    private void ClientLoop()
    {
        Globals.Log("ClientLoop(): Client polling");
        // This is the client's polling loop, which runs continuously on its own thread.
        while (true)
        {
            netmanagerclient?.PollEvents();
            Thread.Sleep(15); // Adjust sleep time to control CPU usage.
        }
    }

    public void Stop()
    {
        netmanagerclient?.Stop();
        Globals.Log("Stop(): Client stopped.");
    }

    public void SendData(string peerIdentifier, string data)
    {
        NetDataWriter writer = new();
        writer.Put(data); // Add your data
        serverPeer?.Send(writer, DeliveryMethod.ReliableOrdered);
        Globals.Log("SendData(): " + peerIdentifier + " Client sent data " + data);
    }

    public void SendAction(PlayerAction action)
    {
        if (action == null || action.ClientIdentifier == null)
            return;
        string peerIdentifier = action.ClientIdentifier;
        Type type = action.GetType();
        action.ClassType = type.FullName;
        Globals.Log("SendAction(): " + action.ClassType);
        dynamic subClassAction = Convert.ChangeType(action, type);

        if (peerIdentifier == null)
        {
            peerIdentifier = subClassAction.ClientIdentifier;
        }
        if (subClassAction.ClientIdentifier == null)
        {
            subClassAction.ClientIdentifier = peerIdentifier;
        }
        String data = JsonSerializer.Serialize(subClassAction);
        SendData(peerIdentifier, data);
    }


    // --- LiteNetLib Event Handlers ---
    private void OnPeerConnected(NetPeer peer)
    {
        Globals.Log($"OnPeerConnected(): Client peer connected: {peer.Address}");
        //clientIdentifier, string classType, string messageAsJson) : base(clientIdentifier, classType, messageAsJson
        //JoinGameAction joinGameAction = new JoinGameAction(ClientIdentifier);
        //joinGameAction.JoinGameValues = JoinGameValues;
        //SendAction(joinGameAction);
    }

    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var jsonString = reader.GetString();
        GameEvent gameEvent = null;
        try 
        {
            gameEvent = JsonSerializer.Deserialize<GameEvent>(jsonString);
        }
        catch(Exception ex)
        {
            Globals.Log("OnNetworkReceive(): Could not deserialize gameEvent: " + ex);
        }
        if (gameEvent != null)
        {
            processGameEvent(gameEvent);
        }    
        reader.Recycle(); // Free up the data reader
    }


    private void processGameEvent(GameEvent gameEvent)
    {
        handleGamePlayEvent(gameEvent);
    }




    private void handleGamePlayEvent(GameEvent gameEvent)
    {
        RurikMonoGame.handleGamePlayEvent(gameEvent);
    }


    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (peer != null)
            Globals.Log($"OnPeerDisconnected(): Client peer disconnected: {peer.Address}. Reason: {disconnectInfo.Reason}");
        // TODO: ReConnect
    }

}
