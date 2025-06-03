using CommandLine;
using parse.project.assets.formatters;

namespace parse.project.assets.Options;

internal class CommandParser
{
    public static RunOptions GetRunOptions(string[] args)
    {
        string packageName = string.Empty;
        string fileName = string.Empty;
        string dotNetVersion = "net8.0";
        int levels = int.MaxValue;
        bool vertical = false;
        FormatOptions format = FormatOptions.text;

        // Resolve the switches with Command Line Parser
        Parser.Default.ParseArguments<ComandOptions>(args)
             .WithParsed(o =>
             {
                 packageName = o.PackageName;
                 fileName = GetFileName(o);

                 if (!string.IsNullOrWhiteSpace(o.Version))
                 {
                     dotNetVersion = o.Version;
                 }
                 if (o.Levels.HasValue)
                 {
                     levels = o.Levels.Value;
                 }
                 vertical = o.Vertical;
                 format = o.Format;
             });

        IOutputFormatter formatter = GetFormatter(format);
        var runOptions = new RunOptions(packageName, fileName, dotNetVersion, levels, format, formatter, vertical);
        return runOptions;
    }

    private static IOutputFormatter GetFormatter(FormatOptions format)
    {
        return format switch
        {
            FormatOptions.text => new TextFormatter(),
            FormatOptions.csv => new CsvFormatter(),
            FormatOptions.mermaid => new MermaidFormatter(),
            _ => throw new Exception($"invalid format {format}"),
        };
    }

    /// <remarks>
    /// I'm trying to be too fancy here. I may be making too many assumptions.
    /// </remarks>
    private static string GetFileName(ComandOptions o)
    {
        string fileName = string.Empty;
        // -If there is no filename, should be `.\project.assets.json`
        if (string.IsNullOrWhiteSpace(o.ProjectAssetsJsonFile))
        {
            string path = Directory.GetCurrentDirectory();
            string tempFileName = Path.Combine(path, "project.assets.json");
            if (File.Exists(tempFileName))
            {
                fileName = tempFileName;
            }
        }
        // - If Path.GetDirectoryName() doesn't find a directory and it ends with .json". Assume that it is a file in the current directory
        else if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(o.ProjectAssetsJsonFile))
            && Path.GetExtension(o.ProjectAssetsJsonFile).ToLower() == ".json")
        {
            string path = Directory.GetCurrentDirectory();
            fileName = Path.Combine(path, o.ProjectAssetsJsonFile);
        }
        // - If it doesn't end with ".json". Assume that this is a directory and the file name is "project.assets.json"
        else if (Path.GetExtension(o.ProjectAssetsJsonFile).ToLower() != ".json")
        {
            fileName = Path.Combine(o.ProjectAssetsJsonFile, "project.assets.json");
        }
        // - Otherwise assume that we have the full directory.
        else
        {
            fileName = o.ProjectAssetsJsonFile;
        }
        return fileName;
    }
}
