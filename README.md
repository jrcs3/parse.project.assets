# parse.project.assets
Inspired by a work situation. 
A code scanner would point out that a specific NuGet package was bad security wise. 
At work we are trying to only replace existing Top Level NuGet packages and only make Transitive package Top Level as a last resort.
I was reading through `project.assets.json` for Dot Net Core apps and though it wouldn't be that hard to automate it.
So here it is.

## Command Line Switches

| Switch              | Required? | Purpose                        | Default / Notes
|---------------------|-----------|--------------------------------|-
| -p  --package       |  X        | The NuGet package to scan for  | 
| -f  --file          |           | The file to scan               | `project.assets.json` in the curent directory
| -d  --dotnetversion |           | .Net Version                   | `net6.0`
| -l  --levels        |           | Levels Deep to graph           | All Levels (`int.MaxValue`)
| -r  --format        |           | Format of the output           | `text` / is case sensitive, only `text`, `csv` and `mermaid`

## Example

```shell
C:\FilesToParse>parse.project.assets -f project.assets.json -p System.Security.Principal.windows

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

- Top? : `X` = This is a Top-level Package 
- Version : The version of this package
- Child : the version requested for the dependency  

## Mermaid


```shell
C:\FilesToParse>parse.project.assets -p System.Security.Principal.windows -r mermaid
flowchart LR
  classDef TopLevel fill: gold
  classDef BottomLevel fill: silver
  MNode1("`System.Security.Principal.Windows
4.7.0`"):::BottomLevel
  MNode1 --> |"`4.7.0`"| MNode2("`Microsoft.Win32.Registry
4.7.0`")
  MNode2 --> |"`4.7.0`"| MNode3{{"`**System.Data.SqlClient**
4.8.5`"}}:::TopLevel
  MNode1 --> |"`4.7.0`"| MNode3
  MNode1 --> |"`4.7.0`"| MNode4("`System.Security.AccessControl
4.7.0`")
  MNode4 --> |"`4.7.0`"| MNode2

C:\FilesToParse>
```

Right now I'm just copying the output of the [Mermaid Live Editor](https://mermaid.live/). When I use that to render the above Mermaid code I get this:

![Left to Right graph of System.Security.Principal.Windows](/assets/images/System.Security.Principal.Windows.png)

### Newtonsoft.Json
Sometimes the graph can get more complex. For example Newtonsoft.Json.

![Left to Right graph of Newtonsoft.Json](/assets/images/Newtonsoft.Json.png)