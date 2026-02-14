using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace rurik;

public class GameEvent
{
    public static readonly string EVENT_TYPE_GAME_STATE_UPDATE ="gameStateUpdate";
    public static readonly string EVENT_TYPE_GAME_STARTED = "gameStarted";
    // Used to send separate message to clients for Events.
    // TODO: Also keep track of these events in a server log.
    public string EventType { get; set; }
    public long Ticks { get; set; }


    public RurikMonoGame? Game { get; set; }
    public ServerGameState? GameState {get; set; }
    public string? EventString { get; set; }

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
            EVENT_TYPE_GAME_STARTED
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



    public void gameStartedHandler()
    {
    }


}
