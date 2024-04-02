using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parse.project.assets.compare;

public class PackageUser
{
    public PackageUser(string name, bool isTopLevel)
    {
        Name = name;
        IsTopLevel = isTopLevel;
    }
    public string Name { get; set; }
    public bool IsTopLevel { get; set; }
}
