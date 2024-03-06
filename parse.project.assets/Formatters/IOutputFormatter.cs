namespace parse.project.assets.Formatters
{
    internal interface IOutputFormatter
    {
        string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels);
        string MakeHead();
        string MakeLine(string packageName, string version, string actualVersion, int tabCount, bool isTopLevel);
        string MakeFooter();
    }
}
