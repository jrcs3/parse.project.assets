﻿using CommandLine;

namespace parse.project.assets.Options;

public class ComandOptions
{
    [Option('p', "package", Required = true, HelpText = "The NuGet package to scan for")]
    public string PackageName { get; set; } = string.Empty;
    [Option('f', "file", Required = false, HelpText = "The file to scan")]
    public string? ProjectAssetsJsonFile { get; set; }
    [Option('d', "dotnetversion", Required = false, HelpText = ".Net Version")]
    public string? Version { get; set; }
    [Option('l', "levels", Required = false, HelpText = "Levels Deep to graph")]
    public int? Levels { get; set; }
    [Option('r', "format", Required = false, HelpText = "Format of the output (text, csv or mermaid)")]
    public FormatOptions Format { get; set; } = FormatOptions.text;
    [Option('v', "vertical", Required = false, HelpText = "Display vertical (mermaid only)")]
    public bool Vertical { get; set; } = false;
}
