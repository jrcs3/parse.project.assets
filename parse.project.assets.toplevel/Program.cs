using Newtonsoft.Json.Linq;
using parse.project.assets.shared.Parse;
using parse.project.assets.shared.Read;
using parse.project.assets.toplevel.Options;

namespace parse.project.assets.toplevel;

internal class Program
{
    private static int Main(string[] args)
    {
        RunOptions runOptions = CommandParser.GetRunOptions(args);

        Console.WriteLine($"{runOptions.FileName}, {runOptions.DotNetVersion}");

        if (!File.Exists(runOptions.FileName))
        {
            Console.WriteLine($"Didn't find the file {runOptions.FileName}");
            return 1;
        }

        FileReader fileReader = new();
        JObject jsonContent = fileReader.ReadFileIntoJObject(runOptions.FileName);

        string dotNetVersion = GetVersion(jsonContent, runOptions.DotNetVersion);

        if (!fileReader.DotNetVersionSupported(dotNetVersion, jsonContent))
        {
            Console.WriteLine($"Didn't find support for the .NET version {runOptions.DotNetVersion}");
            return 1;
        }

        List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, dotNetVersion);
        List<Package> packages = PackageParser.GetPackages(jsonContent, dotNetVersion);
        Console.WriteLine("\r\nTop Level Packages:");
        List<string> packagesOfInterest = new List<string>();
        foreach (Dependency dependency in topDependencies)
        {
            Console.WriteLine($"{dependency.Name} - {dependency.Version}");
            List<Package> flist = packages.Where(x => x.HasDependencyWithName(dependency.Name)).ToList();
            foreach(Package package in flist)
            {
                Dependency deps = package.Dependencies.Where(x => x.Name == dependency.Name).First();
                string verMatch = deps.Version == dependency.Version ? "<= MATCH!" : string.Empty;
                Console.WriteLine($"\t{package.Name} - {package.Version} {verMatch}");
                if (deps.Version == dependency.Version)
                {
                    string packageNameAndVersion = $"{package.Name} - {package.Version}";
                    if (!packagesOfInterest.Contains(packageNameAndVersion))
                    {
                        packagesOfInterest.Add(packageNameAndVersion);
                    }
                }
            }
        }
        if (packagesOfInterest.Count > 0)
        {
            Console.WriteLine("\r\nPackages of Interest:");
            foreach (string package in packagesOfInterest)
            {
                Console.WriteLine(package);
            }
        }

        return 0;
    }

    private static string GetVersion(JObject parsed, string dotNetVersion)
    {
        if (string.IsNullOrWhiteSpace(dotNetVersion))
        {
            JToken items = parsed["projectFileDependencyGroups"];
            if (items.Count() == 1)
            {
                dotNetVersion = ((JProperty)items.First).Name;
            }
        }
        return dotNetVersion;
    }

}
