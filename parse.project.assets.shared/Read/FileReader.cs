using Newtonsoft.Json.Linq;
using parse.project.assets.shared.Parse;

namespace parse.project.assets.shared.Read;

public class FileReader
{
    public JObject ReadFileIntoJObject(string fileName)
    {
        string textContent;
        using (StreamReader r = new(fileName))
        {
            textContent = r.ReadToEnd();
        }

        JObject jsonContent = JObject.Parse(textContent);
        return jsonContent;
    }

    /// <remarks>
    /// Since the tool I'm using to get NuGet packages of interst gives them in all lower case, but 
    /// project.assets.json (and NuGet for that matter) gives package names mixed cases I wanted to 
    /// support prividing package names in any case the user give it in.
    /// Also, it is easier to deal with casing in one place.
    /// </remarks>
    public static string CorrectTarget(string target, List<Package> packages)
    {
        string? correctedName = packages.Where(x => x.Name.Trim().ToLower() == target.Trim().ToLower()).FirstOrDefault()?.Name;
        if (!string.IsNullOrWhiteSpace(correctedName))
        {
            target = correctedName;
        }
        return target;
    }

    public bool DotNetVersionSupported(string dotNetVersion, JObject jsonContent)
    {
        bool projectFileDependencyGroups = ((JObject)jsonContent["projectFileDependencyGroups"]).ContainsKey(dotNetVersion);
        bool targets = ((JObject)jsonContent["targets"]).ContainsKey(dotNetVersion);
        return projectFileDependencyGroups && targets;
    }

}
