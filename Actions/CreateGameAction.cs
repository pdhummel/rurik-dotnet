using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;
namespace rurik.Actions;

public class CreateGameAction : PlayerAction
{
    public CreateGameValues? CreateGameValues { get; set; }

    public CreateGameAction() : base()
    {
    }

    public CreateGameAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        Globals.Log("DeserializeAndExecute(): enter");
        if (MessageAsJson != null)
        {
            CreateGameAction? action =
                    JsonSerializer.Deserialize<CreateGameAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("Execute(): enter");
        if (CreateGameValues == null)
            return;
        Server server = (Server)serverObj;
        Games games = server.Games;
        GameStatus gameStatus = games.CreateGame(CreateGameValues.GameName, CreateGameValues.Owner, CreateGameValues.NumberOfPlayers);
        GameEvent gameEvent = new GameEvent(EVENT_GAME_CREATED);
        gameEvent.GameStatus = gameStatus;
        server.sendGamePlayEvent(gameEvent);
        server.sendGames();
    }
}
