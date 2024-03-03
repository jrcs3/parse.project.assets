using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace parse.project.assets
{
    internal class Dependency
    {
        public Dependency(string name, string version)
        {
            Name = name;
            Version = version;
        }
        public string Name;
        public string Version;
    }
}
