using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using System.Text.Json;
using MonoGame.Extended;
using Myra;
using Myra.Graphics2D.UI;
using static rurik.GameEvent;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Runtime.InteropServices;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Point = Microsoft.Xna.Framework.Point;
using Microsoft.Xna.Framework.Audio;
using static Microsoft.Xna.Framework.Graphics.Texture2D;
using rurik.UI;
namespace rurik;

public class RurikMonoGame : Game
{
    public Server? Server { get; set; }
    public Client? Client { get; set; }
    private GraphicsDeviceManager _graphics;
    private readonly IntPtr drawSurface;
    OrthographicCamera camera;
    long lastMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    public Desktop Desktop { get; set; }

    public Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
    public List<GameEvent> GamePlayEvents { get; set; } = new List<GameEvent>();
    
    // UI Screens
    //public GameLogScreen GameLogScreen { get; set; }
    //public EndGameScreen EndGameScreen { get; set; }
    public GameListScreen GameListScreen { get; set; }

    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MinimizeWindow(IntPtr window);
    //[DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl)]
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool ShowWindow(System.IntPtr hWnd, int nCmdShow);
    private const int SW_SHOWMINIMIZED = 2;


    public RurikMonoGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = Globals.WIDTH;
        _graphics.PreferredBackBufferHeight = Globals.HEIGHT;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Client = new Client(this);
        
        // Initialize UI screens
        //GameLogScreen = new GameLogScreen();
        //EndGameScreen = new EndGameScreen();
    }

    public RurikMonoGame(IntPtr drawSurface) : this()
    {
        this.drawSurface = drawSurface;
        _graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
    }

    void graphics_PreparingDeviceSettings(object? sender, PreparingDeviceSettingsEventArgs e)
    {
        e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = drawSurface;
    }

    public void SetWindowSize(int width, int height)
    {
        Globals.WIDTH = width;
        Globals.HEIGHT = height;
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
    }

    public void minimizeScreen()
    {
        Globals.Log("minimizeScreen(): enter");
        // For some reason ShowWindow does not work.
#if _WINDOWS
        Globals.Log("minimizeScreen(): windows");
        ShowWindow(Window.Handle, SW_SHOWMINIMIZED);
#endif
#if _USE_WINDOWS_FORMS
          Globals.Log("minimizeScreen(): windows forms");
          SDL_MinimizeWindow(Window.Handle);
          Form form = (Form)Control.FromHandle(Window.Handle);
          form.Hide();
#endif
        SetWindowSize(300, 100);
    }

    public void handleGamePlayEvent(GameEvent gameEvent)
    {
        if (gameEvent == null || !gameEvent.IsGamePlayEvent())
            return;
        Globals.Log("handleGamePlayEvent(): gameEvent=" + gameEvent.EventType);
        gameEvent.Ticks = DateTime.Now.Ticks;

        // TOOD
        gameEvent.handleGamePlayEvent(this);
    }


    private void RurikMonoGame_VisibleChanged(object? sender, EventArgs e)
    {
    }

    protected override void Initialize()
    {
        // Add your initialization logic here
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Initialize UI screens
        //GameLogScreen = new GameLogScreen();
        //EndGameScreen = new EndGameScreen();
        setupDesktop();
        GameListScreen = new GameListScreen(this, Desktop);
        GameListScreen.Initialize();
    }


    private void loadSoundEffect(string soundEffectEventName)
    {
        SoundEffect soundEffect = Content.Load<SoundEffect>(soundEffectEventName);
        soundEffects[soundEffectEventName] = soundEffect;
    }

    public void playSoundEffect(string soundEffectEventName)
    {
        if (soundEffects.ContainsKey(soundEffectEventName))
        {
            SoundEffect soundEffect = soundEffects[soundEffectEventName];
            // Volume during playback is scaled by SoundEffect.MasterVolume.
            //soundEffect.Play();
            soundEffect.Play(SoundEffect.MasterVolume, 0.0f, 0.0f);
            Globals.Log("playSoundEffect(): " + soundEffectEventName + ", volume=" + SoundEffect.MasterVolume);
        }
    }


    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    private void setupDesktop()
    {
        Globals.Log("setupDesktop(): enter");
        MyraEnvironment.Game = this;
        Desktop = new Desktop();
        Panel rootPanel = new Panel();
        Desktop.Root = rootPanel;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        if (Desktop != null)
        {
            ShowGameListScreen();
            Desktop.Render();
        }        

        base.Draw(gameTime);
    }

    public void ShowGameListScreen()
    {

        // Show the game list screen
        GameListScreen.Show();
    }



    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        // Update the back buffer size to match the new window size
        _graphics.PreferredBackBufferWidth = Globals.WIDTH;
        _graphics.PreferredBackBufferHeight = Globals.HEIGHT;
        _graphics.ApplyChanges();
    }


}
