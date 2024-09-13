using Newtonsoft.Json.Linq;

namespace parse.project.assets.shared.Options;

public class Tools
{
    public static string GetVersion(JObject parsed, string dotNetVersion)
    {
        if (string.IsNullOrWhiteSpace(dotNetVersion))
        {
            JToken items = parsed["projectFileDependencyGroups"];
            if (items.Count() == 1)
            {
                dotNetVersion = ((JProperty)items.First).Name;
            }
        }
        return dotNetVersion;
    }
}
