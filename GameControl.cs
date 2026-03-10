using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;
using Point = Microsoft.Xna.Framework.Point;
using Myra.Graphics2D.UI;
using Button = Myra.Graphics2D.UI.Button;
using CheckButton = Myra.Graphics2D.UI.CheckButton;
using System.Collections.ObjectModel;

namespace rurik;

public class GameControl
{
    public MouseState previousMouseState = Mouse.GetState();
    public MouseState currentMouseState = Mouse.GetState();
    float clickStartTime;
    bool isMouseDown = false;


    public RurikMonoGame RurikMonoGame { get; set; }
    public GameControl(RurikMonoGame rurikMonoGame)
    {
        RurikMonoGame = rurikMonoGame;
    }
    
    public void Update(GameTime gameTime)
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();

        if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && !isMouseDown)
        {
            //Globals.Log("currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && !isMouseDown");
            isMouseDown = true;
            clickStartTime = (float)gameTime.TotalGameTime.TotalSeconds; // Or use DateTime.Now.Ticks
        }
        else if (isMouseDown && currentMouseState.LeftButton == ButtonState.Pressed &&
                 ((float)gameTime.TotalGameTime.TotalSeconds - clickStartTime >= 1.0f))
        {
            //Globals.Log("currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && !isMouseDown && ((float)gameTime.TotalGameTime.TotalSeconds - clickStartTime >= 1.0f)");
            isMouseDown = false;
            //RurikMonoGame.handleLongLeftClick();
        }
        else if (currentMouseState.LeftButton == ButtonState.Released && isMouseDown)
        {
            //Globals.Log("currentMouseState.LeftButton == ButtonState.Released && isMouseDown");
            isMouseDown = false;
            RurikMonoGame.handleLeftClick(currentMouseState);
        }


        if (currentMouseState.RightButton == ButtonState.Pressed &&
            previousMouseState.RightButton == ButtonState.Released)
        {
            RurikMonoGame.handleRightClick(currentMouseState);
        }

    }
}
