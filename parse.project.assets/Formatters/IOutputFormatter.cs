namespace parse.project.assets.Formatters
{
    internal interface IOutputFormatter
    {
        string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels);
        string MakeHead();
        string MakeLine(string parentPackage, string thisPackage, string thisVersion, string actualVersion, int tabCount, bool isTopLevel);
        string MakeFooter();
    }
}
