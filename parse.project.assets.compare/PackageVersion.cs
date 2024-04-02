﻿namespace parse.project.assets.compare;

public class PackageVersion
{
    public PackageVersion(string versionNumber)
    {
        VersionNumber = versionNumber;
        UsingProject = new List<PackageUser>();
    }
    public string VersionNumber { get; set; }          
    public List<PackageUser> UsingProject { get; set;}
}
