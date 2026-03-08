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
    public Client Client { get; set; }
    private GraphicsDeviceManager _graphics;
    private readonly IntPtr drawSurface;
    OrthographicCamera? camera;
    long lastMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    public Desktop? Desktop { get; set; }

    public Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
    public List<GameEvent> GamePlayEvents { get; set; } = new List<GameEvent>();
    
    public Textures Textures { get; private set; } = new Textures();
    
    // UI Screens
    //public GameLogScreen GameLogScreen { get; set; }
    //public EndGameScreen EndGameScreen { get; set; }
    public GameListScreen? GameListScreen { get; set; }
    public GameSetup? GameSetup { get; set; }
    public MainGameScreen? MainGameScreen { get; set; }
    public string CurrentMyraScreen { get; set; } = "GameListScreen";
    public ChooseFirstPlayerModal? ChooseFirstPlayerModal { get; set; }
    public ChooseLeaderModal? ChooseLeaderModal { get; set; }
    public ChooseSecretAgendaModal? ChooseSecretAgendaModal { get; set; }

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
        Window.ClientSizeChanged += Window_ClientSizeChanged;
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
        GameSetup = new GameSetup(this, Desktop);
        GameSetup.Initialize();
        MainGameScreen = new MainGameScreen(this, Desktop);
        MainGameScreen.Initialize();

        ChooseFirstPlayerModal = new ChooseFirstPlayerModal(this, Desktop);
        ChooseFirstPlayerModal.Initialize();
        ChooseLeaderModal = new ChooseLeaderModal(this, Desktop);
        ChooseLeaderModal.Initialize();
        ChooseSecretAgendaModal = new ChooseSecretAgendaModal(this, Desktop);
        ChooseSecretAgendaModal.Initialize();
        
        // Load textures
        Textures.LoadContent(Content);
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
        //Panel rootPanel = new Panel();
        //Desktop.Root = rootPanel;
        Window window = new Window();
        window.Id = "MyraMainWindow";
        window.Width = Globals.WIDTH;
        window.Height = Globals.HEIGHT;
        Desktop.Root = window;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        if (Desktop != null)
        {
            ShowCurrentScreen();
            Desktop.Render();
        }        

        base.Draw(gameTime);
    }

    public void ShowCurrentScreen()
    {
        if (CurrentMyraScreen == "GameListScreen")
        {
            ShowGameListScreen();
        }
        else if (CurrentMyraScreen == "GameSetup")
        {
            ShowGameSetupScreen();
        }
        else if (CurrentMyraScreen == "MainGameScreen")
        {
            ShowMainGameScreen();
        }
    }

    public void ShowGameListScreen()
    {

        // Show the game list screen
        GameListScreen.Show();
    }

    public void ShowGameSetupScreen()
    {
        // Show the game setup screen
        GameSetup.Show();
    }

    public void ShowMainGameScreen()
    {
        // Show the main game screen
        MainGameScreen.Show();
    }



    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        // Update the back buffer size to match the new window size
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
        _graphics.ApplyChanges();
        if (Desktop != null && Desktop.Root is Window window)
        {
            window.Width = Window.ClientBounds.Width;
            window.Height = Window.ClientBounds.Height;
            if (window.Content != null && window.Content is Panel panel)
            {
                panel.Width = window.Width;
                panel.Height = window.Height;
            }
        }
    }


}
