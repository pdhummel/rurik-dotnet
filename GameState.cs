using System.Text.Json;
using System.Text.Json.Serialization;
namespace rurik;

public class GameState
{
    public string Name { get; set; }
    public List<string> AllowedActions { get; set; }

    public GameState(string name, List<string> allowedActions)
    {
        Name = name;
        AllowedActions = allowedActions;
    }
}


