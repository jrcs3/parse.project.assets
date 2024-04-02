namespace parse.project.assets.compare
{
    public class PackageVersion
    {
        public PackageVersion(string versionNumber)
        {
            VersionNumber = versionNumber;
            UsingProject = new List<string>();
        }
        public string VersionNumber { get; set; }          
        public List<string> UsingProject { get; set;}
    }
}
