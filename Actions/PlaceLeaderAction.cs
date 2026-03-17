using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;

namespace rurik.Actions;

public class PlaceLeaderAction : PlayerAction
{
    public PlaceLeaderValues? PlaceLeaderValues { get; set; }

    public PlaceLeaderAction() : base()
    {
    }

    public PlaceLeaderAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            PlaceLeaderAction? action =
                    JsonSerializer.Deserialize<PlaceLeaderAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("PlaceLeaderAction.Execute(): enter");
        if (PlaceLeaderValues == null || PlaceLeaderValues.GameId == null || PlaceLeaderValues.PlayerColor == null || PlaceLeaderValues.TargetLocation == null)
            return;

        Server server = (Server)serverObj;
        RurikGame game = server.Games.GetGameById(PlaceLeaderValues.GameId);

        if (game == null)
            return;

        // Validate game state - only valid when waitingForLeaderPlacement
        if (game.GameStates.GetCurrentState().Name != "waitingForLeaderPlacement")
        {
            game.Log.AddLogEntry("Cannot place leader: not in waitingForLeaderPlacement state");
            return;
        }

        // Validate player is the current player
        Player player = game.ValidateCurrentPlayer(PlaceLeaderValues.PlayerColor, "PlaceLeaderAction");
        if (player == null)
            return;

        // Validate location exists
        if (!game.GameMap.LocationByName.ContainsKey(PlaceLeaderValues.TargetLocation))
        {
            game.Log.AddLogEntry($"Location not found: {PlaceLeaderValues.TargetLocation}");
            return;
        }

        // Call RurikGame PlaceLeader
        game.PlaceLeader(PlaceLeaderValues.PlayerColor, PlaceLeaderValues.TargetLocation);

        GameStatus gameStatus = server.Games.UpdateGameStatus(PlaceLeaderValues.GameId);
        GameEvent gameEvent = new(EVENT_LEADER_PLACED)
        {
            GameStatus = gameStatus,
            GameMap = game.GameMap
        };
        Globals.Log("PlaceLeaderAction.Execute(): leader in supply=" + gameStatus.Players.playersByColor[PlaceLeaderValues.PlayerColor].supplyLeader);
        server.SendGamePlayEvent(gameEvent);
        server.SendGames();
    }
}
