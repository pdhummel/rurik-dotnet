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
        Server server = (Server)serverObj;
        ServerGameState gameState = server.GameState;
        RurikGame game = server.Games.GetGameById(JoinGameValues.GameId);
        game.JoinGame(JoinGameValues.PlayerName, JoinGameValues.PlayerColor, JoinGameValues.PlayerPosition, false);
        GameStatus gameStatus = server.Games.GetGameStatus(JoinGameValues.GameId);
        GameEvent gameEvent = new GameEvent(EVENT_PLAYER_JOINED_GAME);
        gameEvent.GameStatus = gameStatus;
        server.sendGamePlayEvent(gameEvent);
        server.sendGames();
    }
}
