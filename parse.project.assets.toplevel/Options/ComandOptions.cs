using CommandLine;

namespace parse.project.assets.toplevel.Options;

internal class ComandOptions
{
    [Option('f', "file", Required = false, HelpText = "The file to scan")]
    public string? ProjectAssetsJsonFile { get; set; }
    [Option('d', "dotnetversion", Required = false, HelpText = ".Net Version")]
    public string? Version { get; set; }

}
