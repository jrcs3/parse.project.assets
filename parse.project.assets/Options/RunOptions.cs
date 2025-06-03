using parse.project.assets.formatters;

namespace parse.project.assets.Options;

internal class RunOptions
{
    public RunOptions(string packageName, string fileName, string dotNetVersion, int levels, FormatOptions format, IOutputFormatter formatter, bool vertical)
    {
        PackageName = packageName;
        FileName = fileName;
        DotNetVersion = dotNetVersion;
        Levels = levels;
        Format = format;
        Formatter = formatter;
        Vertical = vertical;
    }

    public string PackageName { get; set; }
    public string FileName { get; set; }
    public string DotNetVersion { get; set; }
    public int Levels { get; set; }
    public FormatOptions Format { get; set; }
    public IOutputFormatter Formatter { get; set; }
    public bool Vertical { get; set; }
}
