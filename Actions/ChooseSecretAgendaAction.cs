using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;

namespace rurik.Actions;

public class ChooseSecretAgendaAction : PlayerAction
{
    public ChooseSecretAgendaValues? ChooseSecretAgendaValues { get; set; }

    public ChooseSecretAgendaAction() : base()
    {
    }

    public ChooseSecretAgendaAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            ChooseSecretAgendaAction? action =
                    JsonSerializer.Deserialize<ChooseSecretAgendaAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("ChooseSecretAgendaAction.Execute(): enter");
        if (ChooseSecretAgendaValues == null || ChooseSecretAgendaValues.GameId == null)
            return;
        
        Server server = (Server)serverObj;
        RurikGame game = server.Games.GetGameById(ChooseSecretAgendaValues.GameId);
        
        if (game == null)
            return;
        
        // Validate game state
        if (game.GameStates.GetCurrentState().Name != "waitingForSecretAgendaSelection")
        {
            game.Log.AddLogEntry("Cannot choose secret agenda: not in waitingForSecretAgendaSelection state");
            return;
        }
        
        // Select the secret agenda
        game.SelectSecretAgenda(ChooseSecretAgendaValues.PlayerColor, ChooseSecretAgendaValues.CardName);
        
        GameStatus gameStatus = server.Games.UpdateGameStatus(ChooseSecretAgendaValues.GameId);
        //gameStatus.ClientPlayer = game.Players.getPlayerByColor(ChooseSecretAgendaValues.PlayerColor);
        GameEvent gameEvent = new(EVENT_SECRET_AGENDA_SELECTED)
        {
            GameStatus = gameStatus,
            GameMap = game.GameMap
        };
        server.SendGamePlayEvent(gameEvent);
        server.SendGames();
    }
}
