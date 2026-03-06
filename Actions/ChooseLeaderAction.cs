using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;
namespace rurik.Actions;

public class ChooseLeaderAction : PlayerAction
{
    public ChooseLeaderValues? ChooseLeaderValues { get; set; }

    public ChooseLeaderAction() : base()
    {
    }

    public ChooseLeaderAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            ChooseLeaderAction? action =
                    JsonSerializer.Deserialize<ChooseLeaderAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("ChooseLeaderAction.Execute(): enter");
        if (ChooseLeaderValues == null || ChooseLeaderValues.GameId == null || ChooseLeaderValues.PlayerColor == null || ChooseLeaderValues.LeaderName == null)
            return;
        
        Server server = (Server)serverObj;
        RurikGame game = server.Games.GetGameById(ChooseLeaderValues.GameId);
        
        if (game == null)
            return;
        
        game.ChooseLeader(ChooseLeaderValues.PlayerColor, ChooseLeaderValues.LeaderName);
        
        GameStatus gameStatus = server.Games.UpdateGameStatus(ChooseLeaderValues.GameId);
        GameEvent gameEvent = new(EVENT_LEADER_CHOSEN)
        {
            GameStatus = gameStatus
        };
        server.SendGamePlayEvent(gameEvent);
        server.SendGames();
    }
}
