# src\Atoms.Core\Atoms.Core.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 4
- **Number of Files**: 65
- **Number of Files with Incidents**: 4
- **Lines of Code**: 2389
- **Estimated LOC to modify**: 10+ (at least 0.4% of the project)

## Related Projects

**Depended on by (4)** — projects that reference this one:

- [C:\Users\paulm\source\repos\atoms-blazor\src\Atoms.Infrastructure\Atoms.Infrastructure.csproj](../projects/Atoms.Infrastructure.md)
- [C:\Users\paulm\source\repos\atoms-blazor\src\Atoms.UseCases\Atoms.UseCases.csproj](../projects/Atoms.UseCases.md)
- [C:\Users\paulm\source\repos\atoms-blazor\src\Atoms.Web\Atoms.Web.csproj](../projects/Atoms.Web.md)
- [C:\Users\paulm\source\repos\atoms-blazor\test\Atoms.UnitTests\Atoms.UnitTests.csproj](../projects/Atoms.UnitTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (4)"]
        P1["<b>📦&nbsp;Atoms.Web.csproj</b><br/><small>net10.0</small>"]
        P3["<b>📦&nbsp;Atoms.UseCases.csproj</b><br/><small>net10.0</small>"]
        P4["<b>📦&nbsp;Atoms.UnitTests.csproj</b><br/><small>net10.0</small>"]
        P5["<b>📦&nbsp;Atoms.Infrastructure.csproj</b><br/><small>net10.0</small>"]
        click P1 "../projects/Atoms.Web.md"
        click P3 "../projects/Atoms.UseCases.md"
        click P4 "../projects/Atoms.UnitTests.md"
        click P5 "../projects/Atoms.Infrastructure.md"
    end
    subgraph current["Atoms.Core.csproj"]
        MAIN["<b>📦&nbsp;Atoms.Core.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/Atoms.Core.md"
    end
    P1 --> MAIN
    P3 --> MAIN
    P4 --> MAIN
    P5 --> MAIN

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 2 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 8 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 2440 |  |
| ***Total APIs Analyzed*** | ***2450*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.AspNetCore.DataProtection.EntityFrameworkCore | 10.0.10 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.10 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.SignalR.Client | 10.0.10 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

