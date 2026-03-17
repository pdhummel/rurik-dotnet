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
    public static readonly string EVENT_SECRET_AGENDA_SELECTED = "secretAgendaSelected";
    public static readonly string EVENT_TROOP_PLACED = "troopPlaced";
    public static readonly string EVENT_LEADER_PLACED = "leaderPlaced";
    public static readonly string EVENT_ADVISOR_PLACED = "advisorPlaced";
    public static readonly string EVENT_LOGIN_SUCCESSFUL = "loginSuccessful";
    public static readonly string EVENT_SERVER_SIDE_MESSAGE = "serverSideMessage";
    
    // Used to send separate message to clients for Events.
    // TODO: Also keep track of these events in a server log.
    public string EventType { get; set; }
    public long Ticks { get; set; }


    public RurikMonoGame? Game { get; set; }
    public ServerGameState? GameState {get; set; }
    public GameStatus? GameStatus {get; set;}
    public AuctionBoard? AuctionBoard {get; set;}
    public GameMap? GameMap {get; set;}
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
            EVENT_GAME_STATE_UPDATE,
            EVENT_SECRET_AGENDA_SELECTED,
            EVENT_TROOP_PLACED,
            EVENT_LEADER_PLACED,
            EVENT_ADVISOR_PLACED,
            EVENT_LOGIN_SUCCESSFUL,
            EVENT_SERVER_SIDE_MESSAGE
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

    public void gameStateUpdateHandler()
    {
        Globals.Log("gameStateUpdateHandler(): enter");    
        //if (Game.ChooseFirstPlayerModal != null)
        //{
        //    Game.ChooseFirstPlayerModal.UpdateGameInfo(GameStatus);
        //}
        //if (Game.GameListScreen != null)
        //    Game.GameListScreen.UpdateGameInfo(GameStatus);
        if (Game.GameSetup != null && Game.GameSetup.IsVisible)
            Game.GameSetup.UpdateGameInfo(GameStatus);
        if (Game.MainGameScreen != null && Game.MainGameScreen.IsVisible)
            Game.MainGameScreen.UpdateGameInfo(GameStatus);
        if (EventString != null && GameStatus.CurrentPlayerName.Equals(Game.Client.ClientIdentifier))
        {
            Game.showMessage(EventString);
        }
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

    public void secretAgendaSelectedHandler()
    {
        Globals.Log("secretAgendaSelectedHandler(): enter");
        if (Game.GameSetup == null)
            return;
        Game.GameSetup.UpdateGameInfo(GameStatus, GameMap);
    }

    public void troopPlacedHandler()
    {
        Globals.Log("troopPlacedHandler(): enter");
        if (Game.MainGameScreen == null)
            return;
        Game.MainGameScreen.UpdateGameInfo(GameStatus, GameMap);
    }

    public void leaderPlacedHandler()
    {
        Globals.Log("leaderPlacedHandler(): enter");
        if (Game.MainGameScreen == null)
            return;
        Globals.Log("leaderPlacedHandler: leader in supply=" + GameStatus.ClientPlayer.supplyLeader);                        
        Game.MainGameScreen.UpdateGameInfo(GameStatus, GameMap);
    }

    public void advisorPlacedHandler()
    {
        Globals.Log("advisorPlacedHandler(): enter");
        if (Game.MainGameScreen == null)
            return;
        GameStatus.AuctionBoard = AuctionBoard;
        Game.MainGameScreen.UpdateGameInfo(GameStatus);
    }


    public void loginSuccessfulHandler()
    {
        Globals.Log("loginSuccessfulHandler(): enter");
        if (Game.GameListScreen != null)
        {
            Game.Client.Games = Games;
            Game.GameListScreen.SuccessfulLogin();
        }
    }

    public void serverSideMessageHandler()
    {
        Globals.Log("serverSideMessageHandler(): enter");
        Game.showMessage(EventString);
    }
}
