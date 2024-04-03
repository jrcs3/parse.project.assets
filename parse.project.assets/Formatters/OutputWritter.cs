using parse.project.assets.shared.Parse;
using System.Text;

namespace parse.project.assets.Formatters;

internal class OutputWritter
{
    /// <remarks>
    /// I'm working from the bottom to the top.
    /// </remarks>
    public static string ParentsStringText(string parentPackage, string thisPackage, List<Package> packages, List<Dependency> topDependencies, string version, int tabCount, int levels, bool vertical, IOutputFormatter formatter)
    {
        if (tabCount > levels)
        {
            return string.Empty;
        }
        StringBuilder sb = new();

        Package? meItem = packages.Where(x => x.Name == thisPackage).ToList().FirstOrDefault();

        string actualVersion = meItem != null ? meItem.Version : string.Empty;
        bool isTopLevel = topDependencies.Where(x => x.Name == thisPackage).Any();
        string thisPackageName = meItem?.Name ?? string.Empty;

        sb.Append(formatter.MakeLine(parentPackage, thisPackageName, version, actualVersion, tabCount, isTopLevel));

        List<Package> flist = packages.Where(x => x.HasDependencyWithName(thisPackage)).ToList();
        foreach (Package p in flist)
        {
            string childVersion = p.Dependencies.Where(x => x.Name == thisPackage).FirstOrDefault()?.Version ?? string.Empty;
            sb.Append(ParentsStringText(thisPackage, p.Name, packages, topDependencies, childVersion, tabCount + 1, levels, vertical, formatter));
        }

        string header = string.Empty;
        if (tabCount == 0 && sb.Length > 0)
        {
            header = formatter.MakeHead(thisPackage, vertical);
        }

        return header + sb.ToString();
    }

    // Experiment: go top to bottom. May need to limit size somehow.
    private static string ChildsStringText(string parentPackage, string thisPackage, List<Package> packages, List<Dependency> topDependencies, string version, int tabCount, int levels, bool vertical, IOutputFormatter formatter)
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
                        sb.Append(ChildsStringText(thisPackage, child.Name, packages, topDependencies, child.Version, tabCount + 1, levels, vertical, formatter));
                    }
                }
            }
        }

        string header = string.Empty;
        if (tabCount == 0 && sb.Length > 0)
        {
            header = formatter.MakeHead(parentPackage, vertical);
        }

        return header + sb.ToString();
    }
}
