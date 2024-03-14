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

        string output = ParentsStringText(string.Empty, runOptions.PackageName, packages, topDependencies, string.Empty, 0, runOptions.Levels, runOptions.Formatter);
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

    // Experiment: go top to bottom. May need to limit size somehow.
    private static string ChildsStringText(string parentPackage, string thisPackage, List<Package> packages, List<Dependency> topDependencies, string version, int tabCount, int levels, IOutputFormatter formatter)
    {
        if (tabCount > levels)
        {
            return string.Empty;
        }
        StringBuilder sb = new();


        Package? meItem = packages.Where(x => x.Name == thisPackage).ToList().FirstOrDefault();
        if (meItem != null)
        {
            string thisPackageName = meItem?.Name ?? string.Empty;
            string actualVersion = meItem != null ? meItem.Version : string.Empty;
            sb.Append(formatter.MakeLine(parentPackage, thisPackageName, version, actualVersion, tabCount, false));
            if (meItem != null)
            {
                List<Dependency> children = meItem.Dependencies;

                if (children != null)
                {
                    foreach (Dependency child in children)
                    {
                        sb.Append(ChildsStringText(thisPackage, child.Name, packages, topDependencies, child.Version, tabCount + 1, levels, formatter));
                    }
                }
            }
        }

        string header = string.Empty;
        if (tabCount == 0 && sb.Length > 0)
        {
            header = formatter.MakeHead();
        }

        return header + sb.ToString();
    }

    /// <remarks>
    /// I'm working from the bottom to the top.
    /// </remarks>
    private static string ParentsStringText(string parentPackage, string thisPackage, List<Package> packages, List<Dependency> topDependencies, string version, int tabCount, int levels, IOutputFormatter formatter)
    {
        if (tabCount > levels)
        {
            return string.Empty;
        }
        StringBuilder sb = new();

        Package? meItem = packages.Where(x => x.Name == thisPackage).ToList().FirstOrDefault();

        string actualVersion = meItem != null ? meItem.Version : string.Empty;
        bool isTopLevel = topDependencies.Where(x => x.Name == thisPackage).Any();
        string thisPackageName = meItem ?.Name ?? string.Empty;

        sb.Append(formatter.MakeLine(parentPackage, thisPackageName, version, actualVersion, tabCount, isTopLevel));

        List<Package> flist = packages.Where(x => x.HasDependencyWithName(thisPackage)).ToList();
        foreach (Package p in flist)
        {
            string childVersion = p.Dependencies.Where(x => x.Name == thisPackage).FirstOrDefault()?.Version ?? string.Empty;
            sb.Append(ParentsStringText(thisPackage, p.Name, packages, topDependencies, childVersion, tabCount + 1, levels, formatter));
        }

        string header = string.Empty;
        if (tabCount == 0 && sb.Length > 0)
        {
            header = formatter.MakeHead();
        }

        return header + sb.ToString();
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