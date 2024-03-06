using CommandLine;
using Newtonsoft.Json.Linq;
using parse.project.assets.Formatters;
using System.Text;

namespace parse.project.assets;

internal class Program
{
    private static int Main(string[] args)
    {
        string packageName = string.Empty;
        string fileName = string.Empty;
        string dotNetVersion = "net6.0";
        int levels = int.MaxValue;
        FormatOptions format = FormatOptions.Text;

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

        string output = ParentsStringText(packageName, packages, topDependencies, string.Empty, 0, levels, formatter);

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
        IOutputFormatter formatter;
        switch (format)
        {
            case FormatOptions.Text:
                formatter = new TextFormatter();
                break;
            case FormatOptions.CSV:
                formatter = new CsvFormatter();
                break;
            default:
                throw new Exception($"invalid format {format}");
        }

        return formatter;
    }

    private static string JobDescription(string packageName, string fileName, string dotNetVersion, int levels)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Package      : {packageName}");
        sb.AppendLine($"File         : {fileName}");
        sb.AppendLine($".NET version : {dotNetVersion}");
        if (levels != int.MaxValue)
        {
            sb.AppendLine($"Levels       : {levels}");
        }
        sb.AppendLine("");
        string superHeader = sb.ToString();
        return superHeader;
    }

    private static string ParentsStringText(string target, List<Package> packages, List<Dependency> topDependencies, string version, int tabCount, int levels, IOutputFormatter formatter)
    {
        //MakeLine makeLine = MakeLineText;
        //MakeHead makeHead = MakeHeaderText;
        //MakeFooter makeFooter = MakeFooterText;

        if (tabCount > levels)
        {
            return string.Empty;
        }
        StringBuilder sb = new();

        Package? meItem = packages.Where(x => x.Name == target).ToList().FirstOrDefault();

        string actualVersion = meItem != null ? meItem.Version : string.Empty;
        bool isTopLevel = topDependencies.Where(x => x.Name == target).Any();

        sb.AppendLine(formatter.MakeLine(target, version, actualVersion, tabCount, isTopLevel));

        var flist = packages.Where(x => x.HasDependencyWithName(target)).ToList();
        foreach (var p in flist)
        {
            string childVersion = p.Dependencies.Where(x => x.Name == target).FirstOrDefault()?.Version ?? string.Empty;
            sb.Append(ParentsStringText(p.Name, packages, topDependencies, childVersion, tabCount + 1, levels, formatter));
        }

        string header = string.Empty;
        string footer = string.Empty;
        if (tabCount == 0 && sb.Length > 0)
        {
            header = formatter.MakeHead();
            footer = formatter.MakeFooter();
        }

        return header + sb.ToString() + footer;
    }

    private static string GetFileName(Options o)
    {
        string fileName = string.Empty;
        if (string.IsNullOrWhiteSpace(o.ProjectAssetsJsonFile))
        {
            string path = Directory.GetCurrentDirectory();
            string tempFileName = Path.Combine(path, "project.assets.json");
            if (File.Exists(tempFileName))
            {
                fileName = tempFileName;
            }
        }
        else if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(o.ProjectAssetsJsonFile)) 
            && Path.GetExtension(o.ProjectAssetsJsonFile).ToLower() == ".json")
        {
            string path = Directory.GetCurrentDirectory();
            fileName = Path.Combine(path, o.ProjectAssetsJsonFile);
        }
        else if (Path.GetExtension(o.ProjectAssetsJsonFile).ToLower() != ".json")
        {
            fileName = Path.Combine(o.ProjectAssetsJsonFile, "project.assets.json");
        }
        else
        {
            fileName = o.ProjectAssetsJsonFile;
        }
        return fileName;
    }

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

    private static List<Dependency> GetTopDependencies(JObject parsed, string dotNetVersion)
    {
        var dIdList = parsed["projectFileDependencyGroups"][dotNetVersion]
            //.Select(x => (JObject)x)
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