using System.Diagnostics;
using System.Text.Json;
using LiteNetLib;
using Microsoft.Xna.Framework;
namespace rurik.Actions;

public class PlayerAction(string clientIdentifier, string classType, string messageAsJson)
{
    public string ClientIdentifier { get; set; } = clientIdentifier; // this is the player name

    public string ClassType { get; set; } = classType;

    public string MessageAsJson { get; set; } = messageAsJson;
    public long Ticks { get; set; } = DateTime.Now.Ticks;

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
        Globals.Log("execute(): " + MessageAsJson);
    }

}