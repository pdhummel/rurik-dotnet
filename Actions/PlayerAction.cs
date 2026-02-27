using System.Diagnostics;
using System.Text.Json;
using LiteNetLib;
using Microsoft.Xna.Framework;
namespace rurik.Actions;

public class PlayerAction
{

    public string? ClientIdentifier { get; set; } // this is the player name

    // Set by Client.SendAction
    public string? ClassType { get; set; }

    // Set by Server.OnNetworkReceive
    public string? MessageAsJson { get; set; }
    public long Ticks { get; set; } = DateTime.Now.Ticks;

    public PlayerAction() {}

    public PlayerAction(string clientIdentifier)
    {
        this.ClientIdentifier = clientIdentifier;
    }

    public PlayerAction? makeSubclass()
    {
        if (ClassType != null)
        {
            Type? type = Type.GetType(ClassType);
            if (type != null)
            {
                object? instance = Activator.CreateInstance(type);
                if (instance != null)
                    return (PlayerAction)instance;
            }
        }
        return null;
    }

    public void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            PlayerAction? action =
                    JsonSerializer.Deserialize<PlayerAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }


    public void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("Execute(): " + MessageAsJson);
    }

}