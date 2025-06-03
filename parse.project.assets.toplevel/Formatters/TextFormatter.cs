using parse.project.assets.toplevel.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parse.project.assets.toplevel.Formatters;

internal class TextFormatter : IOutputFormatter
{
    public string Error(string message)
    {
        return $"{message}";
    }

    public string FileAndVersion(string fileName, string dotNetVersion, bool vertical)
    {
        return $"{fileName} {dotNetVersion}";
    }

    public string MakeHead()
    {
        return "\r\nTop Level Packages:";
    }
}
