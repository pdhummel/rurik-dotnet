namespace rurik;
public class Building
{
    public string color { get; set; }
    public string name { get; set; }

    public Building(string color, string name)
    {
        this.color = color;
        this.name = name;
    }
}