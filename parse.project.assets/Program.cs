using Newtonsoft.Json.Linq;
using parse.project.assets.Formatters;
using parse.project.assets.Options;
using parse.project.assets.Parse;
using System.Text;

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

        JObject jsonContent = ReadFileIntoJObject(runOptions.FileName);

        if (!DotNetVersionSupported(runOptions.DotNetVersion, jsonContent))
        {
            Console.WriteLine($"Didn't find support for the .NET version {runOptions.DotNetVersion}");
            return 1;
        }

        List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, runOptions.DotNetVersion);
        List<Package> packages = PackageParser.GetPackages(jsonContent, runOptions.DotNetVersion);

        runOptions.PackageName = CorrectTarget(runOptions.PackageName, packages);

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

    /// <remarks>
    /// Since the tool I'm using to get NuGet packages of interst gives them in all lower case, but 
    /// project.assets.json (and NuGet for that matter) gives package names mixed cases I wanted to 
    /// support prividing package names in any case the user give it in.
    /// Also, it is easier to deal with casing in one place.
    /// </remarks>
    private static string CorrectTarget(string target, List<Package> packages)
    {
        string? correctedName = packages.Where(x => x.Name.Trim().ToLower() == target.Trim().ToLower()).FirstOrDefault()?.Name;
        if (!string.IsNullOrWhiteSpace(correctedName))
        {
            target = correctedName;
        }
        return target;
    }

    private static bool DotNetVersionSupported(string dotNetVersion, JObject jsonContent)
    {
        bool projectFileDependencyGroups = ((JObject)jsonContent["projectFileDependencyGroups"]).ContainsKey(dotNetVersion);
        bool targets = ((JObject)jsonContent["targets"]).ContainsKey(dotNetVersion);
        return projectFileDependencyGroups && targets;
    }

    private static JObject ReadFileIntoJObject(string jsonStr)
    {
        string textContent;
        using (StreamReader r = new(jsonStr))
        {
            textContent = r.ReadToEnd();
        }

        JObject jsonContent = JObject.Parse(textContent);
        return jsonContent;
    }

}