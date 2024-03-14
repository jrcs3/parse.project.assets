using Newtonsoft.Json.Linq;

namespace parse.project.assets.Parse;

internal class DependencyParser
{
    /// <remarks>
    /// projectFileDependencyGroups contains the Top Level Dependencies for each suppored framework
    /// </remarks>
    public static List<Dependency> GetTopDependencies(JObject parsed, string dotNetVersion)
    {
        List<JToken> dIdList = parsed["projectFileDependencyGroups"][dotNetVersion]
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
}