namespace rurik.Actions;

public class PlaceTroopsValues
{
    public string? GameId { get; set; }
    public string? PlayerColor { get; set; }
    public int TroopCount { get; set; } = 1;
    public string? TargetLocation { get; set; }
}
