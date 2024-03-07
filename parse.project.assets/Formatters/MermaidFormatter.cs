using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parse.project.assets.Formatters
{
    internal class MermaidFormatter : IOutputFormatter
    {
        public MermaidFormatter()
        {
            symbolLookup = new Dictionary<string, string>();
            currentIndex = 0;
        }
        private Dictionary<string, string> symbolLookup;
        private int currentIndex;

        private string GetMermaidSymbol(string packageName, bool isTopLevel, string version= "")
        {
            if (symbolLookup.ContainsKey(packageName))
            {
                return symbolLookup[packageName];
            }
            currentIndex++;
            string symbol = $"K{currentIndex}";
            symbolLookup.Add(packageName, symbol);
            string packageNameDisplay = isTopLevel ? $"**{packageName}**" : packageName;
            string boxOpen = isTopLevel ? "{{" : "(";
            string boxClose = isTopLevel ? "}}" : ")";
            if (string.IsNullOrEmpty(version))
            {
                return $"{symbol}{boxOpen}{packageNameDisplay}{boxClose}";
            }
            return $"{symbol}{boxOpen}\"`{packageNameDisplay}\r\n{version}`\"{boxClose}";
        }



        public string MakeFooter()
        {
            return string.Empty;
        }

        public string MakeHead()
        {
            return "flowchart LR\r\n";
        }

        public string MakeJobDescription(string packageName, string fileName, string dotNetVersion, int levels)
        {
            //if (string.IsNullOrEmpty)
            //throw new NotImplementedException();
            return string.Empty;
        }

        public string MakeLine(string parentPackage, string packageName, string version, string actualVersion, int tabCount, bool isTopLevel)
        {
            if (string.IsNullOrWhiteSpace(parentPackage))
            {
                return GetMermaidSymbol(packageName, isTopLevel, actualVersion);
            }
            return $"{GetMermaidSymbol(parentPackage, isTopLevel)} --> |\"`{version}`\"| {GetMermaidSymbol(packageName, isTopLevel, actualVersion)}"; 
        }
    }
}
