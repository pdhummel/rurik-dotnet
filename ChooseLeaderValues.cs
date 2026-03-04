namespace rurik;

public class ChooseLeaderValues(string gameId, string playerColor, string leaderName)
{
    public string GameId { get; set; } = gameId;
    public string PlayerColor { get; set; } = playerColor;
    public string LeaderName { get; set; } = leaderName;
}
