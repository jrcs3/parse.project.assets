﻿using parse.project.assets.shared.Parse;

namespace parse.project.assets.formatters;

public class CsvFormatter : IOutputFormatter
{
    public string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels)
    {
        return string.Empty;
    }
    public string MakeHead(string packageName, bool vertical, bool groupTopLevel, List<Dependency> topDependencies)
    {
        return $"Level,Package Name,Top?,Version,Child\r\n";
    }

    public string MakeLine(string parentPackage, string packageName, string version, string actualVersion, int tabCount, bool isTopLevel, bool controlsChildVersion)
    {
        //string tabs = tabCount == 0 ? string.Empty : new string(' ', tabCount * 2);
        string topLevelX = isTopLevel ? "X" : string.Empty;
        string stringToAdd = $"{tabCount},\"{packageName}\",{topLevelX},\"{actualVersion}\",\"{version}\"\r\n";
        return stringToAdd;
    }
}
