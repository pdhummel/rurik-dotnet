using System.Text.Json;
using LiteNetLib;
using Microsoft.Xna.Framework;
using rurik;
using static rurik.GameEvent;
namespace rurik.Actions;

public class IdentifyClientAction : PlayerAction
{

    public IdentifyClientAction() : base()
    {}
    
    public IdentifyClientAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        //Globals.Log("DeserializeAndExecute()");
        if (MessageAsJson != null)
        {
            IdentifyClientAction? action =
                    JsonSerializer.Deserialize<IdentifyClientAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("Execute(): ClientIdentifier=" + ClientIdentifier);
        Server server = (Server)serverObj;
        if (server.PlayerNameToPeer.ContainsKey(ClientIdentifier))
        {
            GameEvent gameEvent = new(EVENT_SERVER_SIDE_MESSAGE)
            {
                EventString = ClientIdentifier + " already connected."
            };
            server.SendGamePlayEvent(peer, gameEvent);
            return;
        }

        server.PeerToPlayerName[peer] = ClientIdentifier;
        server.PlayerNameToPeer[ClientIdentifier] = peer;
        server.SendGamePlayEvent(peer, new GameEvent(EVENT_LOGIN_SUCCESSFUL));

    }
}
