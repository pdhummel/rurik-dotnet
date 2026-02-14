using System.Text.Json;
using LiteNetLib;
using rurik;
namespace rurik.Actions;

public class JoinGameAction : PlayerAction
{
    public JoinGameAction(string clientIdentifier, string classType, string messageAsJson) : base(clientIdentifier, classType, messageAsJson)
    {
    }

    public JoinGameValues? JoinGameValues { get; set; }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        //Globals.Log("DeserializeAndExecute()");
        if (MessageAsJson != null)
        {
            JoinGameAction? action =
                    JsonSerializer.Deserialize<JoinGameAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("Execute()");
        Server server = (Server)serverObj;
        ServerGameState gameState = server.GameState;


    }
}
