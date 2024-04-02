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

Console.WriteLine("Proj\tVer\tNuGet");
Console.WriteLine("====\t===\t=====");


foreach (string file in files)
{
    //string projectName = Path.GetFileNameWithoutExtension(file.Replace("obj\\project.assets.json", ""));
    string projectName = GetProjectFromFileName(file);
    Console.WriteLine(projectName);
    JObject jsonContent = fileReader.ReadFileIntoJObject(file);
    List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, dotNetVersion);
    List<Package> packages = PackageParser.GetPackages(jsonContent, dotNetVersion);

    foreach (Package package in packages)
    {
        Console.WriteLine($"\t{package.Version}\t{package.Name}");
        PackageDescription? packageDescription = packageDescriptions.Where(pd => pd.PackageName == package.Name).FirstOrDefault();
        if (packageDescription == null)
        {
            packageDescription =  new PackageDescription(package.Name);
            packageDescriptions.Add(packageDescription);
        }
        PackageVersion? packageVersion = packageDescription.PackageVersions.Where(packageVersion => packageVersion.VersionNumber == package.Version).FirstOrDefault();
        if (packageVersion == null)
        {
            packageVersion = new PackageVersion(package.Version);
            packageDescription.PackageVersions.Add(packageVersion);
        }
        bool isTopLevel = topDependencies.Where(p => p.Name == package.Name).Any();
        PackageUser user = new PackageUser(projectName, isTopLevel);
        packageVersion.UsingProject.Add(user);
    }
}

Console.WriteLine();
Console.WriteLine(new string('-', 80));
Console.WriteLine("  Filter Results");
Console.WriteLine(new string('-', 80));
Console.WriteLine();
Console.WriteLine("NuGet\tVer\tTop?\tProj");
Console.WriteLine("=====\t===\t====\t====");

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
                string isTopLevelString = customer.IsTopLevel ? " X" : " ";
                Console.WriteLine($"\t\t{isTopLevelString}\t{customer.Name}");
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

