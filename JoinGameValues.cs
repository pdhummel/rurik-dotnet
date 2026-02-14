namespace rurik;

public class JoinGameValues(string hostIp, int port, string name)
{
    public int Port { get; set; } = port;

    public string HostIp { get; set; } = hostIp;

    public string Name { get; set; } = name;
}
