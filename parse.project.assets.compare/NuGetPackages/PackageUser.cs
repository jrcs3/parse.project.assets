namespace parse.project.assets.compare.NuGetPackages;

public class PackageUser
{
    public PackageUser(string name, bool isTopLevel)
    {
        Name = name;
        IsTopLevel = isTopLevel;
    }
    public string Name { get; set; }
    public bool IsTopLevel { get; set; }
}
