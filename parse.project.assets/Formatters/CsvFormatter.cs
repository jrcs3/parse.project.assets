namespace parse.project.assets.Formatters
{
    internal class CsvFormatter : IOutputFormatter
    {
        public string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels)
        {
            return string.Empty;
        }
        public string MakeHead()
        {
            return $"Level,Package Name,Top?,Version,Child\r\n";
        }

        public string MakeLine(string packageName, string version, string actualVersion, int tabCount, bool isTopLevel)
        {
            string tabs = tabCount == 0 ? string.Empty : new string(' ', tabCount * 2);
            string topLevelX = isTopLevel ? "X" : string.Empty;
            string stringToAdd = $"{tabCount},{packageName},{topLevelX},{actualVersion},{version}";
            return stringToAdd;
        }
        public string MakeFooter()
        {
            return string.Empty;
        }
    }
}
