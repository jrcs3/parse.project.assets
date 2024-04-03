using System.Text;

namespace parse.project.assets.Formatters
{
    internal class TextFormatter : IOutputFormatter
    {
        public string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels)
        {
            StringBuilder sb = new();
            sb.AppendLine($"Package      : {packageName}");
            sb.AppendLine($"File         : {fileName}");
            sb.AppendLine($".NET version : {dotNetVersion}");
            if (levels != int.MaxValue)
            {
                sb.AppendLine($"Levels       : {levels}");
            }
            sb.AppendLine("");
            string superHeader = sb.ToString();
            return superHeader;
        }

        public string MakeHead(string packageName)
        {
            return $"{string.Empty,-60}\tTop?\tVersion\tChild\r\n{string.Empty,-60}\t====\t=======\t=====\r\n";
        }

        public string MakeLine(string parentPackage, string packageName, string version, string actualVersion, int tabCount, bool isTopLevel)
        {
            string tabs = tabCount == 0 ? string.Empty : new string(' ', tabCount * 2);
            string topLevelX = isTopLevel ? " X" : string.Empty;
            string stringToAdd = $"{tabs}{packageName}".PadRight(60);
            string stringToAddWIthVersions = $"{stringToAdd}\t{topLevelX}\t{actualVersion}\t{version}\r\n";
            return stringToAddWIthVersions;
        }
    }
}
