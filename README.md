# parse.project.assets
Inspired by a work situation. 
A code scanner would point out that a specific NuGet package was bad security wise. 
At work we are trying to only replace existing Top Level NuGet packages and only make Transitive package Top Level as a last resort.
I was reading through `project.assets.json` for Dot Net Core apps and though it wouldn't be that hard to automate it.
So here it is.

## Example

```cmd
C:\FilesToParse>parse.project.assets -f project.assets.json -p System.Security.Principal.windows -l 1

Package      : System.Security.Principal.windows
File         : C:\FilesToParse\project.assets.json
.NET version : net6.0
                                                                Top?    Version Child
                                                                ====    ======= =====
System.Security.Principal.Windows                                       4.7.0
  Microsoft.Win32.Registry                                              4.7.0   4.7.0
    System.Data.SqlClient                                        X      4.8.5   4.7.0
  System.Data.SqlClient                                          X      4.8.5   4.7.0
  System.Security.AccessControl                                         4.7.0   4.7.0
    Microsoft.Win32.Registry                                            4.7.0   4.7.0
      System.Data.SqlClient                                      X      4.8.5   4.7.0

C:\FilesToParse>
```

## Command Line Swithes

| Short | Long            | Required? | Purpose
|-------|-----------------|-----------|-
| -p    | --package       |  X        | The NuGet package to scan for
| -f    | --file          |           | The file to scan
| -d    | --dotnetversion |           | .Net Version
| -l    | --levels        |           | Levels Deep to graph
