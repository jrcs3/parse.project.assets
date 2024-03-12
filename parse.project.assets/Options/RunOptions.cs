using parse.project.assets.Formatters;

namespace parse.project.assets.Options
{
    internal class RunOptions
    {
        public string? PackageName { get; set; }
        public string? FileName { get; set; }
        public string DotNetVersion { get; set; } = "net6.0";
        public int Levels { get; set; }
        public FormatOptions Format { get; set; } = FormatOptions.text;
        public IOutputFormatter? Formatter { get; set; }
    }
}
