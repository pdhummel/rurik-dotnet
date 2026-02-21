using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rurik.UI
{
    public interface IGameScreen
    {
        void Initialize();
        void Update();
        void Draw();
        void Show();
        void Hide();
        void HandleEvent(string eventName, object data);
    }
}
