using parse.project.assets.shared.Parse;

namespace parse.project.assets.formatters;

public interface IOutputFormatter
{
    string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels);
    string MakeHead(string packageName, bool vertical, bool groupTopLevel, List<Dependency> topDependencies);
    string MakeLine(string parentPackage, string thisPackage, string thisVersion, string actualVersion, int tabCount, bool isTopLevel, bool controlsChildVersion);
}
