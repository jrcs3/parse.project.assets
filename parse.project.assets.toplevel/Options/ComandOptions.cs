using CommandLine;

namespace parse.project.assets.toplevel.Options;

internal class ComandOptions
{
    [Option('f', "file", Required = false, HelpText = "The file to scan")]
    public string? ProjectAssetsJsonFile { get; set; }
    [Option('d', "dotnetversion", Required = false, HelpText = ".Net Version")]
    public string? Version { get; set; }
    [Option('r', "format", Required = false, HelpText = "Format of the output (text, csv or mermaid)")]
    public FormatOptions Format { get; set; } = FormatOptions.text;

}
