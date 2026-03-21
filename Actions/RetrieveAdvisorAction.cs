using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;

namespace rurik.Actions;

public class RetrieveAdvisorAction : PlayerAction
{
    public RetrieveAdvisorValues? RetrieveAdvisorValues { get; set; }

    public RetrieveAdvisorAction() : base()
    {
    }

    public RetrieveAdvisorAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            RetrieveAdvisorAction? action =
                    JsonSerializer.Deserialize<RetrieveAdvisorAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("RetrieveAdvisorAction.Execute(): enter");
        if (RetrieveAdvisorValues == null || RetrieveAdvisorValues.GameId == null || RetrieveAdvisorValues.PlayerColor == null || RetrieveAdvisorValues.AdvisorNumber == null || RetrieveAdvisorValues.ActionColumn == null)
            return;

        Server server = (Server)serverObj;
        RurikGame game = server.Games.GetGameById(RetrieveAdvisorValues.GameId);

        if (game == null)
            return;

        // Validate game state - only valid when retrieveAdvisor
        if (game.GameStates.GetCurrentState().Name != "retrieveAdvisor")
        {
            game.Log.AddLogEntry("Cannot retrieve advisor: not in retrieveAdvisor state");
            return;
        }

        // Validate player is the current player
        Player player = game.ValidateCurrentPlayer(RetrieveAdvisorValues.PlayerColor, "RetrieveAdvisorAction");
        if (player == null)
            return;

        try
        {
            // Call RurikGame TakeMainAction
            game.TakeMainAction(
                RetrieveAdvisorValues.PlayerColor,
                RetrieveAdvisorValues.AdvisorNumber,
                RetrieveAdvisorValues.ActionColumn,
                RetrieveAdvisorValues.Row,
                RetrieveAdvisorValues.ForfeitAction
            );

            GameStatus gameStatus = server.Games.UpdateGameStatus(RetrieveAdvisorValues.GameId);
            GameEvent gameEvent = new(EVENT_ADVISOR_RETRIEVED)
            {
                GameStatus = gameStatus
            };
            server.SendGamePlayEvent(gameEvent);
            server.SendGames();
        }
        catch (Exception ex)
        {
            GameEvent gameEvent = new(EVENT_GAME_STATE_UPDATE)
            {
                GameStatus = server.Games.UpdateGameStatus(RetrieveAdvisorValues.GameId),
                EventString = ex.Message
            };
            server.SendGamePlayEvent(gameEvent);
        }
    }
}
