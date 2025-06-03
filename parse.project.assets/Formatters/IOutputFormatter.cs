namespace parse.project.assets.Formatters
{
    public interface IOutputFormatter
    {
        string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels);
        string MakeHead(string packageName, bool vertical);
        string MakeLine(string parentPackage, string thisPackage, string thisVersion, string actualVersion, int tabCount, bool isTopLevel, bool controlsChildVersion);
    }
}
