using CommandLine;

namespace parse.project.assets.compare.Options;

internal class CommandParser
{
    public static RunOptions GetRunOptions(string[] args)
    {
        string rootDir = string.Empty;
        string dotNetVersion = "net6.0";

        // Resolve the switches with Command Line Parser
        Parser.Default.ParseArguments<ComandOptions>(args)
             .WithParsed(o =>
             {
                 rootDir = GetRootDir(o);

                 if (!string.IsNullOrWhiteSpace(o.Version))
                 {
                     dotNetVersion = o.Version;
                 }
             });

        var runOptions = new RunOptions(rootDir, dotNetVersion);
        return runOptions;
    }

    private static string GetRootDir(ComandOptions o)
    {
        
        if (string.IsNullOrWhiteSpace(o.RootDir))
        {
            string path = Directory.GetCurrentDirectory();
            return path;
        }
        return o.RootDir;
    }
}
