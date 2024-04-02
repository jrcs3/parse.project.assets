using Newtonsoft.Json.Linq;
using parse.project.assets.compare;
using parse.project.assets.shared.Parse;
using parse.project.assets.shared.Read;

Console.WriteLine("Comparison app");
FileReader fileReader = new();

string sDir = "C:\\Users\\ajacs\\source\\repos\\parse.project.assets";

string[] files = Directory.GetFiles(sDir, "project.assets.json", SearchOption.AllDirectories).Where(x => x.EndsWith("obj\\project.assets.json")).ToArray();
string dotNetVersion = "net6.0";

List<PackageDescription> packageDescriptions = new List<PackageDescription>();

Console.WriteLine();
Console.WriteLine(new string('-', 80));
Console.WriteLine("  Loading");
Console.WriteLine(new string('-', 80));
Console.WriteLine();


foreach (string file in files)
{
    //string projectName = Path.GetFileNameWithoutExtension(file.Replace("obj\\project.assets.json", ""));
    string projectName = GetProjectFromFileName(file);
    Console.WriteLine(projectName);
    JObject jsonContent = fileReader.ReadFileIntoJObject(file);
    List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, dotNetVersion);
    List<Package> packages = PackageParser.GetPackages(jsonContent, dotNetVersion);

    foreach (Dependency dependency in topDependencies)
    {
        Console.WriteLine($"\t{dependency.Name} => {dependency.Version}");
        PackageDescription? packageDescription = packageDescriptions.Where(pd => pd.PackageName == dependency.Name).FirstOrDefault();
        if (packageDescription == null)
        {
            packageDescription =  new PackageDescription(dependency.Name);
            packageDescriptions.Add(packageDescription);
        }
        PackageVersion? packageVersion = packageDescription.PackageVersions.Where(packageVersion => packageVersion.VersionNumber == dependency.Version).FirstOrDefault();
        if (packageVersion == null)
        {
            packageVersion = new PackageVersion(dependency.Version);
            packageDescription.PackageVersions.Add(packageVersion);
        }
        packageVersion.UsingProject.Add(projectName);
    }
}

Console.WriteLine();
Console.WriteLine(new string('-', 80));
Console.WriteLine("  Filter Results");
Console.WriteLine(new string('-', 80));
Console.WriteLine();

var itemList = packageDescriptions.Where(pd => pd.PackageVersions.Count > 1).ToList();

if (itemList.Count == 0)
{
    Console.WriteLine("Nothing to see here.");
}
else
{
    foreach (PackageDescription packageDescription in itemList)
    {
        Console.WriteLine(packageDescription.PackageName);
        foreach (var packageVersion in packageDescription.PackageVersions)
        {
            Console.WriteLine($"\t{packageVersion.VersionNumber}");
            foreach (var customer in packageVersion.UsingProject)
            {
                Console.WriteLine($"\t\t {customer}");
            }
        }
    }
}

string GetProjectFromFileName(string fileName)
{
    string backClipped = fileName.Replace("\\obj\\project.assets.json", "");
    int pos = backClipped.LastIndexOf('\\');
    return backClipped.Substring(pos + 1);
}

