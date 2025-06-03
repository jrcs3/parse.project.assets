using Newtonsoft.Json.Linq;
using parse.project.assets.formatters;
using parse.project.assets.shared.Parse;
using parse.project.assets.shared.Read;

namespace ParseProjectAssetsUI;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        InitializeWebView2();
    }

    private async void InitializeWebView2()
    {
        await webView21.EnsureCoreWebView2Async();
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {

        OpenFileDialog dlg = new OpenFileDialog();

        dlg.Filter = "C# Projects (*.csproj)|*.csproj|Solution Files (*.sln)|*.sln|All Files (*.*)|*.*";

        dlg.DefaultExt = "csproj";

        dlg.FileName = this.txtFileName.Text ?? string.Empty;

        var result = dlg.ShowDialog();

        if (result == DialogResult.OK)
        {
            SetFileNameAndLoadPackageList(dlg.FileName);
        }
    }
    private void btnParse_Click(object sender, EventArgs e)
    {
        string fileName = txtFileName.Text;
        string fileExtention = Path.GetExtension(fileName).ToLowerInvariant();
        switch (fileExtention)
        {
            case ".csproj":
                ParseCsProj(fileName);
                break;
            case ".sln":
                ParseSolution(fileName);
                break;
            default:
                MessageBox.Show("Unsupported file type. Please select a .csproj or .sln file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                break;
        }
    }

    private void SetFileNameAndLoadPackageList(string fileName)
    {
        if (fileName != null && txtFileName.Text != fileName)
        {
            txtFileName.Text = fileName;
            JObject jsonContent = LoadJsonContent(fileName);

            string dotNetVersion = GetVersion(jsonContent, string.Empty);

            List<Package> packages = PackageParser.GetPackages(jsonContent, dotNetVersion);

            txtPackageName.Items.Clear();
            foreach (var package in packages)
            {
                if (!string.IsNullOrWhiteSpace(package.Name))
                {
                    txtPackageName.Items.Add(package.Name);
                }
            }
        }
    }

    private static JObject LoadJsonContent(string fileName)
    {
        string filePath = Path.GetDirectoryName(fileName);
        string projectAssetsJsonPath = Path.Combine(filePath, "obj", "project.assets.json");

        FileReader fileReader = new();
        JObject jsonContent = fileReader.ReadFileIntoJObject(projectAssetsJsonPath);
        return jsonContent;
    }

    private void ParseSolution(string fileName)
    {
        throw new NotImplementedException();
    }

    private void ParseCsProj(string fileName)
    {
        string filePath = Path.GetDirectoryName(fileName);
        string projectName = Path.GetFileNameWithoutExtension(fileName);

        bool isVertial = false;

        string packageName = txtPackageName.Text.Trim();

        FileReader fileReader = new();
        JObject jsonContent = LoadJsonContent(fileName);

        string dotNetVersion = GetVersion(jsonContent,string.Empty);

        List<Dependency> topDependencies = DependencyParser.GetTopDependencies(jsonContent, dotNetVersion);
        List<Package> packages = PackageParser.GetPackages(jsonContent, dotNetVersion);

        packageName = FileReader.CorrectTarget(packageName, packages);

        IOutputFormatter FormatOptions= new MermaidFormatter();

        string output = OutputWritter.ParentsStringText(string.Empty, packageName, packages, topDependencies, string.Empty, 0, int.MaxValue, false, isVertial, FormatOptions);

        string htmlContent = @"<html>
  <body>
    <h2>{ProjectName}</h2>
    <pre class=""mermaid"">
           {Content}
    </pre>

    <script type=""module"">
      import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.esm.min.mjs';
      mermaid.initialize({ startOnLoad: true });
    </script>
  </body>
</html>";

        webView21.NavigateToString(htmlContent.Replace("{Content}", output).Replace("{ProjectName}", projectName));
    }

    private static string GetVersion(JObject parsed, string dotNetVersion)
    {
        if (string.IsNullOrWhiteSpace(dotNetVersion))
        {
            var items = parsed["projectFileDependencyGroups"];
            if (items.Count() == 1)
            {
                dotNetVersion = ((JProperty)items.First).Name;
            }
        }
        return dotNetVersion;
    }

}
