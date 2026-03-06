using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using rurik.UI;

namespace rurik;

public class GameEvent
{
    public static readonly string EVENT_GAME_STATE_UPDATE ="gameStateUpdate";
    public static readonly string EVENT_GAMES_UPDATE ="gamesUpdate";
    public static readonly string EVENT_GAME_CREATED = "gameCreated";
    public static readonly string EVENT_GAME_STARTED = "gameStarted";
    public static readonly string EVENT_PLAYER_JOINED_GAME = "playerJoinedGame";
    public static readonly string EVENT_LEADER_CHOSEN = "leaderChosen";
    public static readonly string EVENT_FIRST_PLAYER_SELECTED = "firstPlayerSelected";
    
    // Used to send separate message to clients for Events.
    // TODO: Also keep track of these events in a server log.
    public string EventType { get; set; }
    public long Ticks { get; set; }


    public RurikMonoGame? Game { get; set; }
    public ServerGameState? GameState {get; set; }
    public GameStatus? GameStatus {get; set;}
    public Games? Games {get;set;}
    public string? EventString { get; set; }
    public string? PlayerName { get; set; }

    private int secondsForPopupToAppear = 10;
    



    public HashSet<string> GamePlayEvents { get; set; } = new HashSet<string>();


    public GameEvent(string eventType)
    {
        EventType = eventType;
        initializeGamePlayEvents();
    }

    private void initializeGamePlayEvents()
    {
        var gamePlayEvents = new string[]
        {
            EVENT_GAME_STARTED,
            EVENT_GAMES_UPDATE,
            EVENT_GAME_CREATED,
            EVENT_PLAYER_JOINED_GAME,
            EVENT_LEADER_CHOSEN,
            EVENT_FIRST_PLAYER_SELECTED,
            EVENT_GAME_STATE_UPDATE
        };
        GamePlayEvents.UnionWith(gamePlayEvents);
    }

    public bool IsGamePlayEvent()
    {
        bool isGamePlayEvent = false;
        if (GamePlayEvents.Contains(EventType))
            isGamePlayEvent = true;
        return isGamePlayEvent;
    }

    public void handleGamePlayEvent(RurikMonoGame game)
    {
        Game = game;
        MethodInfo eventMethodHandler = this.GetType().GetMethod(EventType + "Handler");
        object[] parameters = new object[] { };
        try
        {
            Globals.Log("handleGamePlayEvent(): " + EventType);
            Thread eventMethodHandlerThread = new Thread(() => eventMethodHandler?.Invoke(this, parameters));
            eventMethodHandlerThread.IsBackground = true;
            eventMethodHandlerThread.Start();
            //eventMethodHandler?.Invoke(this, parameters);
        }
        catch (Exception ex)
        {
            Globals.Log("handleGamePlayEvent(): " + EventType + ", Exception: " + ex);
        }

    }

    public void gamesUpdateHandler()
    {
        Globals.Log("gamesUpdateHandler(): enter");
        Game.Client.Games = Games;
        Game.GameListScreen.RefreshGameList();
    }

    public void gameCreatedHandler()
    {
        Globals.Log("gameCreatedHandler(): enter");
        if (GameStatus == null)
            return;
        if (GameStatus.Owner.Equals(Game.Client.ClientIdentifier))
            Game.GameListScreen.JoinGameAfterGameCreated(GameStatus);
        else
            Game.GameListScreen.RefreshGameList();
    }


    public void gameStartedHandler()
    {
    }

    public void playerJoinedGameHandler()
    {
        Globals.Log("playerJoinedGameHandler(): enter");
        if (Game.GameListScreen != null)
            Game.GameListScreen.RefreshGameList();
        if (Game.GameSetup != null)
            Game.GameSetup.UpdateGameInfo(GameStatus);
        
        // Open the game setup screen for the player who joined
        if (PlayerName.Equals(Game.Client.ClientIdentifier))
        {
            Game.GameListScreen.OpenGameSetup(GameStatus);
        }
    }

    public void firstPlayerSelectedHandler()
    {
        Globals.Log("firstPlayerSelectedHandler(): enter");
        if (Game.GameSetup == null)
            return;
        Game.GameSetup.UpdateGameInfo(GameStatus);
    }

    public void leaderChosenHandler()
    {
        Globals.Log("leaderChosenHandler(): enter");
        if (Game.GameSetup == null)
            return;
        Game.GameSetup.UpdateGameInfo(GameStatus);
    }


    public void gameStateUpdateHandler()
    {
        Globals.Log("gameStateUpdateHandler(): enter");    
        if (Game.ChooseFirstPlayerModal != null)
        {
            Game.ChooseFirstPlayerModal.UpdateGameInfo(GameStatus);
        }
    }

}
