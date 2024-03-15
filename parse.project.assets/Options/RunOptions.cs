using parse.project.assets.Formatters;

namespace parse.project.assets.Options
{
    internal class RunOptions
    {
        public RunOptions(string packageName, string fileName, string dotNetVersion, int levels, FormatOptions format, IOutputFormatter formatter)
        {
            PackageName = packageName;
            FileName = fileName;
            DotNetVersion = dotNetVersion;
            Levels = levels;
            Format = format;
            Formatter = formatter;
        }

        public string PackageName { get; set; }
        public string FileName { get; set; }
        public string DotNetVersion { get; set; }
        public int Levels { get; set; }
        public FormatOptions Format { get; set; }
        public IOutputFormatter Formatter { get; set; }
    }
}
