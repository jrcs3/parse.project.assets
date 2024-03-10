using CommandLine;
using Newtonsoft.Json.Linq;
using parse.project.assets.Formatters;
using System.Text;

namespace parse.project.assets;

internal class Program
{
    private static int Main(string[] args)
    {
        // Potential command line switches
        string packageName = string.Empty;
        string fileName = string.Empty;
        string dotNetVersion = "net6.0";
        int levels = int.MaxValue;
        FormatOptions format = FormatOptions.text;

        // Resolve the switches with Command Line Parser
        Parser.Default.ParseArguments<Options>(args)
             .WithParsed(o =>
             {
                 packageName = o.PackageName;
                 fileName = GetFileName(o);

                 if (!string.IsNullOrWhiteSpace(o.Version))
                 {
                     dotNetVersion = o.Version;
                 }
                 if (o.Levels.HasValue)
                 {
                     levels = o.Levels.Value;
                 }
                 format = o.Format;
             });

        IOutputFormatter formatter = GetFormatter(format);

        Console.Write(formatter.MakeJobDescription(packageName, fileName, dotNetVersion, levels));

        if (!File.Exists(fileName))
        {
            Console.WriteLine($"Didn't find the file {fileName}");
            return 1;
        }

        JObject jsonContent = ReadFileIntoJObject(fileName);

        if (!DotNetVersionSupported(dotNetVersion, jsonContent))
        {
            Console.WriteLine($"Didn't find support for the .NET version {dotNetVersion}");
            return 1;
        }

        List<Dependency> topDependencies = GetTopDependencies(jsonContent, dotNetVersion);
        List<Package> packages = GetPackages(jsonContent, dotNetVersion);

        packageName = CorrectTarget(packageName, packages);

        string output = ParentsStringText(string.Empty, packageName, packages, topDependencies, string.Empty, 0, levels, formatter);

        if (string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine($"Nothing found for {packageName}");
            return 1;
        }

        if (levels < 1)
        {
            Console.WriteLine($"Invalid Levels, {levels} doesn't make sense.");
            return 1;
        }

        Console.Write(output);

        return 0;
    }

    private static IOutputFormatter GetFormatter(FormatOptions format)
    {
        switch (format)
        {
            case FormatOptions.text:
                return new TextFormatter();
            case FormatOptions.csv:
                return new CsvFormatter();
            case FormatOptions.mermaid:
                return new MermaidFormatter();
            default:
                throw new Exception($"invalid format {format}");
        }
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

        var flist = packages.Where(x => x.HasDependencyWithName(thisPackage)).ToList();
        foreach (var p in flist)
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
    /// I'm trying to be too fancy here. I may be making too many assumptions.
    /// </remarks>
    private static string GetFileName(Options o)
    {
        string fileName = string.Empty;
        // -If there is no filename, should be `.\project.assets.json`
        if (string.IsNullOrWhiteSpace(o.ProjectAssetsJsonFile))
        {
            string path = Directory.GetCurrentDirectory();
            string tempFileName = Path.Combine(path, "project.assets.json");
            if (File.Exists(tempFileName))
            {
                fileName = tempFileName;
            }
        }
        // - If Path.GetDirectoryName() doesn't find a directory and it ends with .json". Assume that it is a file in the current directory
        else if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(o.ProjectAssetsJsonFile)) 
            && Path.GetExtension(o.ProjectAssetsJsonFile).ToLower() == ".json")
        {
            string path = Directory.GetCurrentDirectory();
            fileName = Path.Combine(path, o.ProjectAssetsJsonFile);
        }
        // - If it doesn't end with ".json". Assume that this is a directory and the file name is "project.assets.json"
        else if (Path.GetExtension(o.ProjectAssetsJsonFile).ToLower() != ".json")
        {
            fileName = Path.Combine(o.ProjectAssetsJsonFile, "project.assets.json");
        }
        // - Otherwise assume that we have the full directory.
        else
        {
            fileName = o.ProjectAssetsJsonFile;
        }
        return fileName;
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

        var jsonContent = JObject.Parse(textContent);
        return jsonContent;
    }

    /// <remarks>
    /// projectFileDependencyGroups contains the Top Level Dependencies for each suppored framework
    /// </remarks>
    private static List<Dependency> GetTopDependencies(JObject parsed, string dotNetVersion)
    {
        var dIdList = parsed["projectFileDependencyGroups"][dotNetVersion]
            .ToList();

        List<Dependency> topDependencies = new();

        foreach (JToken? d in dIdList)
        {
            string fullName = d.ToString();

            string[] parts = fullName.Split(">=");

            if (parts.Length > 1)
            {
                string name = parts[0].Trim();
                string version = parts[1].Trim();
                topDependencies.Add(new Dependency(name, version));
            }
        }
        return topDependencies;
    }

    /// <remarks>
    /// targets contains the Top Level Dependencies for each suppored framework
    /// </remarks>
    private static List<Package> GetPackages(JObject parsed, string dotNetVersion)
    {
        List<JProperty> dIdList = parsed["targets"][dotNetVersion]
            .Select(x => (JProperty)x)
            .ToList();

        List<Package> packages = new();

        foreach (var d in dIdList)
        {
            string fullName = d.Name;
            if (!string.IsNullOrEmpty(fullName))
            {
                string[] parts = fullName.Split('/');
                if (parts.Length > 1)
                {
                    string name = parts[0];
                    string version = parts[1];

                    var pack = new Package(name, version);

                    var dependency = d.Select(x => x.Value<JObject>("dependencies")).FirstOrDefault();

                    if (dependency != null)
                    {
                        foreach (var ddws in dependency.Properties())
                        {
                            var dname = ddws.Name;
                            var dversion = ddws.Value.ToString();

                            pack.Dependencies.Add(new Dependency(dname, dversion));
                        }
                    }
                    packages.Add(pack);
                }
            }
        }
        return packages;
    }
}