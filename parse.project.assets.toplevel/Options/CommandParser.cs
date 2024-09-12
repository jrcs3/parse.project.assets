using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace parse.project.assets.toplevel.Options;

internal class CommandParser
{
    internal static RunOptions GetRunOptions(string[] args)
    {
        string fileName = string.Empty;
        string dotNetVersion = string.Empty;

        Parser.Default.ParseArguments<ComandOptions>(args)
             .WithParsed(o =>
             {
                 fileName = GetFileName(o);

                 if (!string.IsNullOrWhiteSpace(o.Version))
                 {
                     dotNetVersion = o.Version;
                 }
             });

        var runOptions = new RunOptions(fileName, dotNetVersion);
        return runOptions;

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
