using Newtonsoft.Json.Linq;
using parse.project.assets.Formatters;
using parse.project.assets.Options;
using parse.project.assets.shared.Parse;
using parse.project.assets.shared.Read;

namespace parse.project.assets;

internal class Program
{
    private static int Main(string[] args)
    {
        RunOptions runOptions = CommandParser.GetRunOptions(args);

        Console.Write(runOptions.Formatter.MakeJobDescription(runOptions.PackageName, runOptions.FileName, runOptions.DotNetVersion, runOptions.Levels));

        if (!File.Exists(runOptions.FileName))
        {
            Console.WriteLine($"Didn't find the file {runOptions.FileName}");
            return 1;
        }

        FileReader fileReader = new();
        JObject jsonContent = fileReader.ReadFileIntoJObject(runOptions.FileName);

        if (!fileReader.DotNetVersionSupported(runOptions.DotNetVersion, jsonContent))
        {
            Console.WriteLine($"Didn't find support for the .NET version {runOptions.DotNetVersion}");
            return 1;
        }

        List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, runOptions.DotNetVersion);
        List<Package> packages = PackageParser.GetPackages(jsonContent, runOptions.DotNetVersion);

        runOptions.PackageName = fileReader.CorrectTarget(runOptions.PackageName, packages);

        string output = OutputWritter.ParentsStringText(string.Empty, runOptions.PackageName, packages, topDependencies, string.Empty, 0, runOptions.Levels, runOptions.Formatter);
        //string output = ChildsStringText(string.Empty, runOptions.PackageName, packages, topDependencies, string.Empty, 0, runOptions.Levels, runOptions.Formatter);

        if (string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine($"Nothing found for {runOptions.PackageName}");
            return 1;
        }

        if (runOptions.Levels < 1)
        {
            Console.WriteLine($"Invalid Levels, {runOptions.Levels} doesn't make sense.");
            return 1;
        }

        Console.Write(output);

        return 0;
    }
}