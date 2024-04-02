namespace parse.project.assets.shared.Parse;

public class Package
{
    public Package(string name, string version)
    {
        Name = name;
        Version = version;
        Dependencies = new List<Dependency>();
    }
    public string Name;
    public string Version;
    public List<Dependency> Dependencies;

    public bool HasDependencyWithName(string name)
    {
        return Dependencies.Where(x => x.Name == name).Any();
    }
}
