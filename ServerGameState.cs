using System.Text.Json;
using System.Text.Json.Serialization;
namespace rurik;

public class ServerGameState(GameSettings settings)
{
    [JsonPropertyName("GS")]
    public GameSettings GameSettings { get; set; } = settings;
    public string Version { get; set; } = "v0.0.1";

    public override string ToString()
    {
        string returnString = "GameState: " + ToJson();
        return returnString;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

}
