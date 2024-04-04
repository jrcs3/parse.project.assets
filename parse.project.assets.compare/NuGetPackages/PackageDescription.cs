namespace parse.project.assets.compare.NuGetPackages;

public class PackageDescription
{
    public PackageDescription(string packageName)
    {
        PackageName = packageName;
        PackageVersions = new List<PackageVersion>();
    }
    public string PackageName;
    public List<PackageVersion> PackageVersions { get; set; }
}
