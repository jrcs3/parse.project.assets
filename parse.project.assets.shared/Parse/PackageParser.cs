using Newtonsoft.Json.Linq;

namespace parse.project.assets.shared.Parse;

public class PackageParser
{
    /// <remarks>
    /// targets contains the Top Level Dependencies for each suppored framework
    /// </remarks>
    public static List<Package> GetPackages(JObject parsed, string dotNetVersion)
    {
        List<JProperty> dIdList = parsed["targets"][dotNetVersion]
            .Select(x => (JProperty)x)
            .ToList();

        List<Package> packages = new();

        foreach (JProperty d in dIdList)
        {
            string fullName = d.Name;
            if (!string.IsNullOrEmpty(fullName))
            {
                string[] parts = fullName.Split('/');
                if (parts.Length > 1)
                {
                    string name = parts[0];
                    string version = parts[1];

                    Package pack = new(name, version);

                    JObject? dependency = d.Select(x => x.Value<JObject>("dependencies")).FirstOrDefault();

                    if (dependency != null)
                    {
                        foreach (JProperty? ddws in dependency.Properties())
                        {
                            string dname = ddws.Name;
                            string dversion = ddws.Value.ToString();

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