// See https://aka.ms/new-console-template for more information
using CommandLine;
using Newtonsoft.Json.Linq;
using parse.project.assets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace parse.project.assets;

internal class Program
{
    private static void Main(string[] args)
    {
        //string target = "ADO.Net.Client";
        string target = string.Empty;// = "System.Security.Principal.Windows";


        string jsonStr = string.Empty;// = "C:\\Users\\ajacs\\source\\repos\\parse.project.assets\\parse.project.assets\\FilesToParse\\project.assets.json";
        //var jsonStr = "\\FilesToParse\\project.assets.json";


        Parser.Default.ParseArguments<Options>(args)
             .WithParsed<Options>(o =>
             {
                 target = o.PackageName;
                 jsonStr = o.ProjectAssetsJsonFile;
             });

        string json;
        using (StreamReader r = new StreamReader(jsonStr))
        {
            json = r.ReadToEnd();
        }

        var parsed = JObject.Parse(json);

        List<Package> packages = GetPackages(parsed);

        List<Dependency> topDependencies = GetTopDependencies(parsed);

        //foreach (Dependency dependency in topDependencies)
        //{
        //    Console.WriteLine($"{dependency.Name} {dependency.Version}");
        //}


        //Console.WriteLine($"Looking for {target}\r\n");

        //var flist = packages.Where(x => x.HasDependencyWithName(target)).ToList();

        ////foreach (var p in packages)
        //foreach (var p in flist)
        //{
        //    Console.WriteLine($"{p.Name} {p.Version}");
        //    foreach (var dd in p.Dependencies)
        //    {
        //        Console.WriteLine($"\t{dd.Name} {dd.Version}");
        //    }
        //}

        Console.WriteLine($"Target: {target}");
        Console.WriteLine($"Project File: {jsonStr}");
        Console.WriteLine("");

        //Console.WriteLine("");
        //Console.WriteLine(new string('=', 50));
        //Console.WriteLine("");

        string output = ParentsString(target, packages, topDependencies, string.Empty, 0);

        Console.WriteLine($"{string.Empty.PadRight(60)}\tExpect\tActual\tTop?");
        Console.WriteLine($"{string.Empty.PadRight(60)}\t======\t======\t====");
        Console.Write(output);

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
            sb.AppendLine("\t X");
        }
        else
        {
            sb.AppendLine("");
            var flist = packages.Where(x => x.HasDependencyWithName(target)).ToList();
            foreach (var p in flist)
            {
                sb.Append(ParentsString(p.Name, packages, topDependencies, p.Version, tabCount + 1));
            }
        }
        return sb.ToString();
    }

    static List<Dependency> GetTopDependencies(JObject parsed)
    {
        var dIdList = parsed["projectFileDependencyGroups"]["net6.0"]
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

    static List<Package> GetPackages(JObject parsed)
    {
        List<JProperty> dIdList = parsed["targets"]["net6.0"]
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