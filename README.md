# parse.project.assets
Inspired by a work situation. 
A code scanner would point out that a specific NuGet package was bad security wise. 
At work we are trying to only replace existing Top Level NuGet packages and only make Transitive package Top Level as a last resort.
I was reading through `project.assets.json` for Dot Net Core apps and though it wouldn't be that hard to automate it.
So here it is.