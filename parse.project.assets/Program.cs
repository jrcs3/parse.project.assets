// See https://aka.ms/new-console-template for more information
using CommandLine;
using Newtonsoft.Json.Linq;
using System.Text;

namespace parse.project.assets;

internal class Program
{
    private static int Main(string[] args)
    {
        string target = string.Empty;
        string jsonStr = string.Empty;
        string dotNetVersion = "net6.0";

        Parser.Default.ParseArguments<Options>(args)
             .WithParsed<Options>(o =>
             {
                 target = o.PackageName;
                 jsonStr = o.ProjectAssetsJsonFile;
                 if (!string.IsNullOrWhiteSpace(o.Version))
                 {
                     dotNetVersion = o.Version;
                 }
             });

        Console.WriteLine($"Target: {target}");
        Console.WriteLine($"Project File: {jsonStr}");
        Console.WriteLine($".NET version: {dotNetVersion}");
        Console.WriteLine("");

        if (!File.Exists(jsonStr))
        {
            Console.WriteLine($"Didn't find the file {jsonStr}");
            return 1;
        }

        JObject jsonContent = ReadFileIntoJObject(jsonStr);

        if (!DotNetVersionSupported(dotNetVersion, jsonContent))
        {
            Console.WriteLine($"Didn't find support for the .NET version {dotNetVersion}");
            return 1;
        }

        List<Dependency> topDependencies = GetTopDependencies(jsonContent, dotNetVersion);

        List<Package> packages = GetPackages(jsonContent, dotNetVersion);

        target = CorrectTarget(target, packages);

        string output = ParentsString(target, packages, topDependencies, string.Empty, 0);

        if (string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine($"Nothing found for {target}");
            return 1;
        }

        Console.WriteLine($"{string.Empty.PadRight(60)}\tExpect\tActual\tTop?");
        Console.WriteLine($"{string.Empty.PadRight(60)}\t======\t======\t====");
        Console.Write(output);

        return 0;
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
        using (StreamReader r = new StreamReader(jsonStr))
        {
            textContent = r.ReadToEnd();
        }

        var jsonContent = JObject.Parse(textContent);
        return jsonContent;
    }

    static string ParentsString(string target, List<Package> packages, List<Dependency> topDependencies, string version, int tabCount)
    {
        StringBuilder sb = new();
        string tabs = tabCount == 0 ? string.Empty : new string(' ', tabCount * 4);
        Package? meItem = packages.Where(x => x.Name == target).ToList().FirstOrDefault();

        string actualVersion = meItem != null ? meItem.Version : string.Empty;

        string stringToAdd = $"{tabs}{target}".PadRight(60);
        sb.Append($"{stringToAdd}\t{version}\t{actualVersion}");
        System.Diagnostics.Debug.WriteLine($"{target}\t{tabCount}");

        if (topDependencies.Where(x => x.Name == target).Any())
        {
            sb.Append("\t X");
        }
        sb.AppendLine("");
        var flist = packages.Where(x => x.HasDependencyWithName(target)).ToList();
        foreach (var p in flist)
        {
            sb.Append(ParentsString(p.Name, packages, topDependencies, p.Version, tabCount + 1));
        }
        return sb.ToString();
    }

    static List<Dependency> GetTopDependencies(JObject parsed, string dotNetVersion)
    {
        var dIdList = parsed["projectFileDependencyGroups"][dotNetVersion]
            //.Select(x => (JObject)x)
            .ToList();

        List<Dependency> topDependencies = new List<Dependency>();

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

    static List<Package> GetPackages(JObject parsed, string dotNetVersion)
    {
        List<JProperty> dIdList = parsed["targets"][dotNetVersion]
            .Select(x => (JProperty)x)
            .ToList();

        List<Package> packages = new List<Package>();

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