namespace parse.project.assets.toplevel.Options;

internal class RunOptions
{
    string _fileName;
    string _dotNetVersion;
    internal RunOptions(string fileName, string dotNetVersion)
    {
        _fileName = fileName;
        _dotNetVersion = dotNetVersion;
    }

    public string FileName { get { return _fileName; } }
    public string DotNetVersion { get { return _dotNetVersion; } }
}
