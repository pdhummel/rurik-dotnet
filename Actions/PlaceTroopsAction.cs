using System.Text.Json;
using LiteNetLib;
using rurik;
using static rurik.GameEvent;

namespace rurik.Actions;

public class PlaceTroopsAction : PlayerAction
{
    public PlaceTroopsValues? PlaceTroopsValues { get; set; }

    public PlaceTroopsAction() : base()
    {
    }

    public PlaceTroopsAction(string clientIdentifier) : base(clientIdentifier)
    {
    }

    public new void DeserializeAndExecute(NetPeer peer, Object serverObj)
    {
        if (MessageAsJson != null)
        {
            PlaceTroopsAction? action =
                    JsonSerializer.Deserialize<PlaceTroopsAction>(this.MessageAsJson);
            action?.Execute(peer, serverObj);
        }
    }

    public new void Execute(NetPeer peer, Object serverObj)
    {
        Globals.Log("PlaceTroopsAction.Execute(): enter");
        if (PlaceTroopsValues == null || PlaceTroopsValues.GameId == null || PlaceTroopsValues.PlayerColor == null || PlaceTroopsValues.TargetLocation == null)
            return;

        Server server = (Server)serverObj;
        RurikGame game = server.Games.GetGameById(PlaceTroopsValues.GameId);

        if (game == null)
            return;

        // Validate game state - only valid when waitingForTroopPlacement
        if (game.GameStates.GetCurrentState().Name != "waitingForTroopPlacement")
        {
            game.Log.AddLogEntry("Cannot place troops: not in waitingForTroopPlacement state");
            return;
        }

        // Validate player is the current player
        Player player = game.ValidateCurrentPlayer(PlaceTroopsValues.PlayerColor, "PlaceTroopsAction");
        if (player == null)
            return;

        // Validate player has troops available to place
        if (player.TroopsToDeploy < PlaceTroopsValues.TroopCount)
        {
            game.Log.AddLogEntry($"Cannot place {PlaceTroopsValues.TroopCount} troops: player only has {player.TroopsToDeploy} troops available");
            return;
        }

        // Validate location exists
        if (!game.GameMap.LocationByName.ContainsKey(PlaceTroopsValues.TargetLocation))
        {
            game.Log.AddLogEntry($"Location not found: {PlaceTroopsValues.TargetLocation}");
            return;
        }

        // Call RurikGame PlaceInitialTroop
        game.PlaceInitialTroop(PlaceTroopsValues.PlayerColor, PlaceTroopsValues.TargetLocation);

        GameStatus gameStatus = server.Games.UpdateGameStatus(PlaceTroopsValues.GameId);
        GameEvent gameEvent = new(EVENT_TROOP_PLACED)
        {
            GameStatus = gameStatus,
            GameMap = game.GameMap
        };
        server.SendGamePlayEvent(gameEvent);
        server.SendGames();
    }
}
