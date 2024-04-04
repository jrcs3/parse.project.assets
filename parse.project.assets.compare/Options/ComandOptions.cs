using CommandLine;

namespace parse.project.assets.compare.Options;

internal class ComandOptions
{

    [Option('r', "rootDir", Required = false, HelpText = ".Net Version")]
    public string? RootDir { get; set; }
    [Option('d', "dotnetversion", Required = false, HelpText = ".Net Version")]
    public string? Version { get; set; }

}
