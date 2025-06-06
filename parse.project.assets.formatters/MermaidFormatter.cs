﻿using parse.project.assets.shared.Parse;

namespace parse.project.assets.formatters;

public class MermaidFormatter : IOutputFormatter
{
    public MermaidFormatter()
    {
        _symbolLookup = new Dictionary<string, string>();
        _currentIndex = 0;
        _duplicatesList = new List<string>();
    }
    private readonly Dictionary<string, string> _symbolLookup;
    private int _currentIndex;
    private readonly List<string> _duplicatesList;

    private const string _seperator = "<br/>";

    private string GetMermaidSymbol(string packageName, bool isTopLevel, string version= "")
    {
        if (_symbolLookup.ContainsKey(packageName))
        {
            return $"{_symbolLookup[packageName]}";
        }
        _currentIndex++;
        string symbol = $"MNode{_currentIndex}";
        _symbolLookup.Add(packageName, symbol);
        string packageNameDisplay = isTopLevel ? $"**{packageName}**" : packageName;
        string boxOpen = isTopLevel ? "{{" : "(";
        string boxClose = isTopLevel ? "}}:::TopLevel" : ")";
        if (string.IsNullOrEmpty(version))
        {
            return $"{symbol}{boxOpen}{packageNameDisplay}{boxClose}";
        }
        return $"{symbol}{boxOpen}\"`{packageNameDisplay}{_seperator}{version}`\"{boxClose}";
    }

    #region Prevent duplicate connections
    private static string FormatDuplicatesListData(string parentPackage, string thisPackage, string actualVersion)
    {
        return $"{parentPackage}--{thisPackage}--{actualVersion}";
    }

    private bool IsDuplicate(string parentPackage, string thisPackage, string actualVersion)
    {
        return _duplicatesList.Contains(FormatDuplicatesListData(parentPackage, thisPackage, actualVersion));
    }

    private void AddToDuplicateList(string parentPackage, string thisPackage, string actualVersion)
    {
        string line = FormatDuplicatesListData(parentPackage, thisPackage, actualVersion);
        if (!_duplicatesList.Contains(line))
        {
            _duplicatesList.Add(line);
        }
    }
    #endregion Prevent duplicate connections

    public string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels)
    {
        return string.Empty;
    }

    public string MakeHead(string packageName, bool vertical, bool groupTopLevel, List<Dependency> topDependencies)
    {
        string directionSymbol = vertical ? "TB" : "LR";
        string returnValue = $"---\r\ntitle: {packageName}\r\n---\r\nflowchart {directionSymbol}\r\n  classDef TopLevel fill: gold, color: black\r\n  classDef TargetLevel fill: silver, color: black\r\n";

        if (groupTopLevel)
        {
            returnValue += "\r\n  subgraph \"Top Level Packages\"\r\n";
            foreach (Dependency dep in topDependencies)
            {
                if (_symbolLookup.ContainsKey(dep.Name))
                {
                    returnValue += $"      {GetMermaidSymbol(dep.Name, true)}\r\n";
                }
            }
            returnValue += "  end\r\n\r\n";
        }
        return returnValue;
    }

    public string MakeLine(string parentPackage, string packageName, string version, string actualVersion, int tabCount, bool isTopLevel, bool controlsChildVersion)
    {
        if (IsDuplicate(parentPackage, packageName, actualVersion))
        {
            return string.Empty;
        }
        AddToDuplicateList(parentPackage, packageName, actualVersion);
        string lineSegment = controlsChildVersion ? "==>" : "-.-> ";
        if (string.IsNullOrWhiteSpace(parentPackage))
        {
            string className = (tabCount == 0 && !isTopLevel) ? ":::TargetLevel" : string.Empty;
            return $"  {GetMermaidSymbol(packageName, isTopLevel, actualVersion)}{className}\r\n";
        }
        return $"  {GetMermaidSymbol(parentPackage, isTopLevel)} {lineSegment} |\"`{version}`\"| {GetMermaidSymbol(packageName, isTopLevel, actualVersion)}\r\n"; 
    }
}
