using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;
namespace rurik.Actions;

public class ChooseFirstPlayerAction : PlayerAction
{
    public ChooseFirstPlayerValues? ChooseFirstPlayerValues { get; set; }

    public ChooseFirstPlayerAction() : base()
    {
    }

    public ChooseFirstPlayerAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            ChooseFirstPlayerAction? action =
                    JsonSerializer.Deserialize<ChooseFirstPlayerAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("ChooseFirstPlayerAction.Execute(): enter");
        if (ChooseFirstPlayerValues == null || ChooseFirstPlayerValues.GameId == null)
            return;
        
        Server server = (Server)serverObj;
        RurikGame game = server.Games.GetGameById(ChooseFirstPlayerValues.GameId);
        
        if (game == null)
            return;
        
        // Validate game state
        if (game.GameStates.GetCurrentState().Name != "waitingForFirstPlayerSelection")
        {
            game.Log.AddLogEntry("Cannot choose first player: not in waitingForFirstPlayerSelection state");
            return;
        }
        
        // If player color is specified, select that player
        if (!string.IsNullOrEmpty(ChooseFirstPlayerValues.PlayerColor))
        {
            game.SelectFirstPlayer(ChooseFirstPlayerValues.PlayerColor);
        }
        else
        {
            // Otherwise select a random player
            game.SelectRandomFirstPlayer();
        }
        game.StartGame();
        
        GameStatus gameStatus = server.Games.UpdateGameStatus(ChooseFirstPlayerValues.GameId);
        GameEvent gameEvent = new(EVENT_FIRST_PLAYER_SELECTED)
        {
            GameStatus = gameStatus
        };
        server.SendGamePlayEvent(gameEvent);
        server.SendGames();
    }
}
