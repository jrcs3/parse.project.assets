using Newtonsoft.Json.Linq;
using parse.project.assets.shared.Parse;
using parse.project.assets.shared.Read;
using parse.project.assets.toplevel.Options;
using System.Text;
using parse.project.assets.shared.Options;
using parse.project.assets.toplevel.Formatters;

namespace parse.project.assets.toplevel;

internal class Program
{
    private static int Main(string[] args)
    {
        RunOptions runOptions = CommandParser.GetRunOptions(args);

        IOutputFormatter outputFormatter = new TextFormatter();


        Console.WriteLine(outputFormatter.FileAndVersion(runOptions.FileName, runOptions.DotNetVersion, false));

        if (!File.Exists(runOptions.FileName))
        {
            Console.WriteLine(outputFormatter.Error($"Didn't find the file {runOptions.FileName}"));
            return 1;
        }

        FileReader fileReader = new();
        JObject jsonContent = fileReader.ReadFileIntoJObject(runOptions.FileName);

        string dotNetVersion = Tools.GetVersion(jsonContent, runOptions.DotNetVersion);

        if (!fileReader.DotNetVersionSupported(dotNetVersion, jsonContent))
        {
            Console.WriteLine(outputFormatter.Error($"Didn't find support for the .NET version {runOptions.DotNetVersion}"));
            return 1;
        }

        List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, dotNetVersion);
        List<Package> packages = PackageParser.GetPackages(jsonContent, dotNetVersion);
        Console.WriteLine(WriteOutput(topDependencies, packages, outputFormatter));

        return 0;
    }

    private static string WriteOutput(List<Dependency> topDependencies, List<Package> packages, IOutputFormatter outputFormatter)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(outputFormatter.MakeHead());
        // List of top-level packages that we want to investigate.
        List<string> packagesOfInterest = new List<string>();
        // Go through all the top-level packages
        foreach (Dependency dependency in topDependencies)
        {
            sb.AppendLine($"{dependency.Name} - {dependency.Version}");
            // Get all the packages that reference this top-level package.
            List<Package> flist = packages.Where(x => x.HasDependencyWithName(dependency.Name)).ToList();
            foreach (Package package in flist)
            {
                // Get the version asked for (may be lower than actual version)
                Dependency deps = package.Dependencies.Where(x => x.Name == dependency.Name).First();
                string verMatch = deps.Version == dependency.Version ? "<= MATCH!" : string.Empty;
                sb.AppendLine($"\t{package.Name} - {package.Version} {verMatch}");
                // If another package refereces this package, and it is the same version.
                // We may not need to make this a top-level package
                if (deps.Version == dependency.Version)
                {
                    // Avoid duplcation
                    string packageNameAndVersion = $"{package.Name} - {package.Version}";
                    if (!packagesOfInterest.Contains(packageNameAndVersion))
                    {
                        packagesOfInterest.Add(packageNameAndVersion);
                    }
                }
            }
        }
        // If there are any top-level packages to investigate, list them here.
        if (packagesOfInterest.Count > 0)
        {
            sb.AppendLine("\r\nPackages of Interest:");
            foreach (string package in packagesOfInterest)
            {
                sb.AppendLine(package);
            }
        }
        return sb.ToString();
    }
}
