namespace parse.project.assets.compare.Options;

internal class RunOptions
{
    public RunOptions(string rootDir, string dotNetVersion)
    {
        RootDir = rootDir;
        DotNetVersion = dotNetVersion;
    }

    public string RootDir { get; set; }
    public string DotNetVersion { get; set; }
}