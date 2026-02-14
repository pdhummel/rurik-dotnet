namespace rurik;
public class Leader
{
    public string name { get; set; }
    public string description { get; set; }

    public Leader(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
}
