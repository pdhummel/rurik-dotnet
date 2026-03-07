namespace rurik;

public class ChooseSecretAgendaValues(string gameId, string playerColor, string cardName)
{
    public string GameId { get; set; } = gameId;
    public string PlayerColor { get; set; } = playerColor;
    public string CardName { get; set; } = cardName;
}
