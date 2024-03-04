using CommandLine;

namespace parse.project.assets;

public class Options
{
    [Option('p', "package", Required = true, HelpText = "The package to scan for")]
    public string PackageName { get; set; }

    [Option('f', "file", Required = true, HelpText = "The file to scan")]
    public string ProjectAssetsJsonFile { get; set; }
    [Option('v', "version", Required = false, HelpText = ".Net Version")]
    public string? Version { get; set; }
}
