namespace parse.project.assets.shared.Parse;

public class Dependency
{
    public Dependency(string name, string version)
    {
        Name = name;
        Version = version;
    }
    public string Name;
    public string Version;
}
