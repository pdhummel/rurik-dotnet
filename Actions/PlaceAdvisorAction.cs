using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;

namespace rurik.Actions;

public class PlaceAdvisorAction : PlayerAction
{
    public PlaceAdvisorValues? PlaceAdvisorValues { get; set; }

    public PlaceAdvisorAction() : base()
    {
    }

    public PlaceAdvisorAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            PlaceAdvisorAction? action =
                    JsonSerializer.Deserialize<PlaceAdvisorAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("PlaceAdvisorAction.Execute(): enter");
        if (PlaceAdvisorValues == null || PlaceAdvisorValues.GameId == null || PlaceAdvisorValues.PlayerColor == null || PlaceAdvisorValues.ActionColumn == null || PlaceAdvisorValues.Advisor == null)
            return;

        Server server = (Server)serverObj;
        RurikGame game = server.Games.GetGameById(PlaceAdvisorValues.GameId);

        if (game == null)
            return;

        // Validate game state - only valid when strategyPhase
        if (game.GameStates.GetCurrentState().Name != "strategyPhase")
        {
            game.Log.AddLogEntry("Cannot place advisor: not in strategyPhase state");
            return;
        }

        // Validate player is the current player
        Player player = game.ValidateCurrentPlayer(PlaceAdvisorValues.PlayerColor, "PlaceAdvisorAction");
        if (player == null)
            return;

        // Call RurikGame PlayAdvisor
        game.PlayAdvisor(PlaceAdvisorValues.PlayerColor, PlaceAdvisorValues.ActionColumn, PlaceAdvisorValues.Advisor, PlaceAdvisorValues.BidCoins);

        GameStatus gameStatus = server.Games.UpdateGameStatus(PlaceAdvisorValues.GameId, PlaceAdvisorValues.PlayerColor);
        GameEvent gameEvent = new(EVENT_ADVISOR_PLACED)
        {
            GameStatus = gameStatus,
            AuctionBoard = game.AuctionBoard
        };
        server.SendGamePlayEvent(gameEvent);
        server.SendGames();
    }
}
