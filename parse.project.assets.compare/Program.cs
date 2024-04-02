// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;
using parse.project.assets.shared.Parse;
using System.Net.Http.Json;

Console.WriteLine("Hello, World!");

string sDir = "C:\\Users\\ajacs\\source\\repos\\parse.project.assets";

string[] files = Directory.GetFiles(sDir, "project.assets.json", SearchOption.AllDirectories).Where(x => x.EndsWith("obj\\project.assets.json")).ToArray();


foreach (string file in files)
{
    Console.WriteLine(file);
    JObject jsonContent = ReadFileIntoJObject(file);
    List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, "net6.0");
    foreach (Dependency dependency in topDependencies)
    {
        Console.WriteLine($"\t{dependency.Name} => {dependency.Version}");
    }
}



static JObject ReadFileIntoJObject(string jsonStr)
{
    string textContent;
    using (StreamReader r = new(jsonStr))
    {
        textContent = r.ReadToEnd();
    }

    JObject jsonContent = JObject.Parse(textContent);
    return jsonContent;
}

