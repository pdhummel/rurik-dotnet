using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;
namespace rurik.Actions;

public class JoinGameAction : PlayerAction
{

    public JoinGameAction() : base()
    {}
    
    public JoinGameAction(string clientIdentifier) : base(clientIdentifier)
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
        if (JoinGameValues == null || JoinGameValues.GameId == null)
            return;
        Server server = (Server)serverObj;
        //server.PeerToPlayerName[peer] = JoinGameValues.PlayerName;
        //server.PlayerNameToPeer[JoinGameValues.PlayerName] = peer;
        RurikGame game = server.Games.GetGameById(JoinGameValues.GameId);
        game.JoinGame(JoinGameValues.PlayerName, JoinGameValues.PlayerColor, JoinGameValues.PlayerPosition, false);
        GameStatus gameStatus = server.Games.UpdateGameStatus(JoinGameValues.GameId);
        GameEvent gameEvent = new(EVENT_PLAYER_JOINED_GAME)
        {
            GameStatus = gameStatus,
            PlayerName = JoinGameValues.PlayerName,
        };
        server.SendGamePlayEvent(gameEvent);
        server.SendGames();
    }
}
