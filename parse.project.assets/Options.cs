using CommandLine;

namespace parse.project.assets;

public class Options
{
    [Option('p', "package", Required = true, HelpText = "The NuGet package to scan for")]
    public string PackageName { get; set; } = string.Empty;
    [Option('f', "file", Required = false, HelpText = "The file to scan")]
    public string? ProjectAssetsJsonFile { get; set; }
    [Option('d', "dotnetversion", Required = false, HelpText = ".Net Version")]
    public string? Version { get; set; }
    [Option('l', "levels", Required = false, HelpText = "Levels Deep to graph")]
    public int? Levels { get; set; }
}
