using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parse.project.assets.toplevel.Formatters;

public interface IOutputFormatter
{
    string MakeHead();

    string FileAndVersion(string fileName, string dotNetVersion, bool vertical);
    string Error(string message);
}
