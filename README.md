# parse.project.assets
Inspired by a work situation. 
A code scanner would point out that a specific NuGet package was bad security wise. 
At work we are trying to only replace existing Top Level NuGet packages and only make Transitive package Top Level as a last resort.
I was reading through `project.assets.json` for Dot Net Core apps and though it wouldn't be that hard to automate it.
So here it is.

## Command Line Switches

| Switch              |  Purpose                        | Default / Notes
|---------------------|---------------------------------|-
| -p  --package       | The NuGet package to scan for   | / **REQUIRED**. 
| -f  --file          | The file to scan                | `project.assets.json` in the curent directory
| -d  --dotnetversion | .Net Version                    | `net6.0`
| -l  --levels        | Levels Deep to graph            | All Levels (`int.MaxValue`)
| -r  --format        | Format of the output            | `text` / is case sensitive, only `text`, `csv` and `mermaid`

## Example


### Levels

`-l 1` on the command line. Gives you only one level deep, useful if you want to see the see which version of the 
package is requested by each caller (this information is present without the switch, it is just easier to read with it). 

```shell
C:\FilesToParse>parse.project.assets -f project.assets.json -p System.Security.Principal.windows

Package      : System.Security.Principal.Windows
File         : C:\FilesToParse\project.assets.json
.NET version : net6.0
Levels       : 1

                                                                Top?    Version Child
                                                                ====    ======= =====
System.Security.Principal.Windows                                       4.7.0
  Microsoft.Win32.Registry                                              4.7.0   4.7.0
  System.Data.SqlClient                                          X      4.8.5   4.7.0
  System.Security.AccessControl                                         4.7.0   4.7.0

C:\FilesToParse>_
```

### Format

The process is the same. It is just that a different `IOutputFormatter` class is used to render the output.

#### Text

`-r text` on the command line. Rendered by *TextFormatter.cs*
 
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

C:\FilesToParse>_
```

- Top? : `X` = This is a Top-level Package 
- Version : The version of this package
- Child : the version requested for the dependency  

#### CSV

`-r csv` on the command line. Rendered by *CsvFormatter.cs*

```shell
C:\FilesToParse>parse.project.assets -f project.assets.json -p System.Security.Principal.windows
Level,Package Name,Top?,Version,Child
0,"System.Security.Principal.Windows",,"4.7.0",""
1,"Microsoft.Win32.Registry",,"4.7.0","4.7.0"
2,"System.Data.SqlClient",X,"4.8.5","4.7.0"
1,"System.Data.SqlClient",X,"4.8.5","4.7.0"
1,"System.Security.AccessControl",,"4.7.0","4.7.0"
2,"Microsoft.Win32.Registry",,"4.7.0","4.7.0"
3,"System.Data.SqlClient",X,"4.8.5","4.7.0"
C:\FilesToParse>_
```

- Level : How many degrees of seperation between this package and the -p package (0 being self)
- Package Name : The name of the NuGet package
- Top? : `X` = This is a Top-level Package 
- Version : The version of this package
- Child : the version requested for the dependency  

#### Mermaid

`-r mermaid` on the command line. Rendered by *MermaidFormatter.cs*. 

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

C:\FilesToParse>_
```

You can change the `RL` to `TB` if you want to 

Right now I'm just copying the output of the [Mermaid Live Editor](https://mermaid.live/). When I use that to render the above Mermaid code I get this:

![Left to Right graph of System.Security.Principal.Windows](/assets/images/System.Security.Principal.Windows.png)

### Newtonsoft.Json
Sometimes the graph can get more complex. For example Newtonsoft.Json.

![Left to Right graph of Newtonsoft.Json](/assets/images/Newtonsoft.Json.png)