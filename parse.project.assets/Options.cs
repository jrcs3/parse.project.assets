using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace parse.project.assets;

public class Options
{
    [Option('p', "package", Required = true, HelpText = "The package to scan for.")]
    public string PackageName { get; set; }

    [Option('f', "file", Required = true, HelpText = "The file to scan.")]
    public string ProjectAssetsJsonFile { get; set; }
}
