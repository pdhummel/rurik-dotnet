using Myra.Events;
using Myra.Graphics2D.UI;
using System;

namespace rurik.UI
{
    public class WindowPanel
    {
        private Window _window;
        private RurikMonoGame _rurikMonoGame;
        private Desktop _desktop;
        private Panel _panel;
        private bool _isVisible;
        private bool _isMinimized = false;
        private int _width;
        private int _height;

        public bool IsVisible
        {
            get => _isVisible;
            private set => _isVisible = value;
        }

        public WindowPanel(RurikMonoGame rurikMonoGame, Desktop desktop, Panel panel, string title, int height, int width)
        {
            initialize(rurikMonoGame, desktop, panel, title, height, width);
        }

        public WindowPanel(RurikMonoGame rurikMonoGame, Desktop desktop, Panel panel, string title)
        {
            initialize(rurikMonoGame, desktop, panel, title, 0, 0);
        }

        private void initialize(RurikMonoGame rurikMonoGame, Desktop desktop, Panel panel, string title, int height, int width)
        {
            Globals.Log("WindowPanel(): " + title);
            _rurikMonoGame = rurikMonoGame;
            _desktop = desktop;
            _panel = panel;
            
            // Create a new window with the specified title
            _window = new Window
            {
                Title = title,
                Content = panel
            };
            if (height > 0)
            {
                _window.Height = height;
            }
            if (width > 0)
            {
                _window.Width = width;
            }
            _window.Closing += (s, a) =>
            {
                Globals.Log("Closing(): enter");
                CancellableEventArgs args = (CancellableEventArgs)a;
                if (this._isMinimized)
                    Restore();
                else
                    Minimize();
                args.Cancel = true;
            };
        }

        public void Show()
        {
            Globals.Log("Show(): enter");
            _isVisible = true;
            
            // Remove the window from its parent if it exists
            try
            {
                _window.RemoveFromParent();
            }
            catch (Exception)
            {
                // Ignore if already removed
            }

            // Set the window content and show it
            _window.Content = _panel;
            _window.Show(_desktop);
        }

        public void Hide()
        {
            Globals.Log("Hide(): enter");
            _isVisible = false;
            _window.Close();
        }

        public void Close()
        {
            Globals.Log("Close(): enter");
            _isVisible = false;
            _window.Close();
        }

        public void Minimize()
        {
            Globals.Log("Minimize(): enter");
            // Minimize the window by setting its height to just the title bar
            // In Myra, we can set the window to a collapsed state
            if (_window.Height != null)
                _height = (int)_window.Height;
            else if (_window.MinHeight != null)
                _height = (int)_window.MinHeight;
            else if (_window.MaxHeight != null)
                _height = (int)_window.MaxHeight;
            else if (_window.Content != null && _window.Content.Height != null)
                _height = (int)_window.Content.Height;
            //_window.Height = 30;
            //((Panel)_window.Content).Height = 0;
            _window.Content = null;
            _isMinimized = true;
            Globals.Log("Minimize(): exit");
        }

        public void Restore()
        {
            Globals.Log("Restore(): enter");
            // Restore the window to its full size
            _window.Content = _panel;
            //((Panel)_window.Content).Widgets[0].Height;
            if (_height > 0)
                _window.Height = _height;
            _isMinimized = false;
            Globals.Log("Restore(): exit");
        }

        public void Update()
        {
            Globals.Log("Update(): enter");
            // Update logic if needed
        }
    }
}
