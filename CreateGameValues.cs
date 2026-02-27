namespace rurik;

public class CreateGameValues(string gameName, string owner)
{

    public string Owner { get; set; } = owner;

    public string GameName { get; set; } = gameName;
    public int NumberOfPlayers {get; set;} = 4;
}
