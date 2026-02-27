namespace rurik;

public class JoinGameValues(string gameId, string playerName, string playerColor, string playerPosition)
{
    public string GameId {get; set;} = gameId;
    public string PlayerName { get; set; } = playerName;
    public string PlayerColor { get; set; } = playerColor;
    public string PlayerPosition { get; set; } = playerPosition;
}
