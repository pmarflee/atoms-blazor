# src\Atoms.Infrastructure\Atoms.Infrastructure.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 3
- **Dependants**: 2
- **Number of Files**: 43
- **Number of Files with Incidents**: 4
- **Lines of Code**: 3949
- **Estimated LOC to modify**: 9+ (at least 0.2% of the project)

## Related Projects

**Depends on (3)** — projects this one references:

- [C:\Users\paulm\source\repos\atoms-blazor\src\Atoms.Components\Atoms.Components.csproj](../projects/Atoms.Components.md)
- [C:\Users\paulm\source\repos\atoms-blazor\src\Atoms.Core\Atoms.Core.csproj](../projects/Atoms.Core.md)
- [C:\Users\paulm\source\repos\atoms-blazor\src\Atoms.UseCases\Atoms.UseCases.csproj](../projects/Atoms.UseCases.md)

**Depended on by (2)** — projects that reference this one:

- [C:\Users\paulm\source\repos\atoms-blazor\src\Atoms.Web\Atoms.Web.csproj](../projects/Atoms.Web.md)
- [C:\Users\paulm\source\repos\atoms-blazor\test\Atoms.UnitTests\Atoms.UnitTests.csproj](../projects/Atoms.UnitTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (2)"]
        P1["<b>📦&nbsp;Atoms.Web.csproj</b><br/><small>net10.0</small>"]
        P4["<b>📦&nbsp;Atoms.UnitTests.csproj</b><br/><small>net10.0</small>"]
        click P1 "../projects/Atoms.Web.md"
        click P4 "../projects/Atoms.UnitTests.md"
    end
    subgraph current["Atoms.Infrastructure.csproj"]
        MAIN["<b>📦&nbsp;Atoms.Infrastructure.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/Atoms.Infrastructure.md"
    end
    subgraph downstream["Dependencies (3)"]
        P7["<b>📦&nbsp;Atoms.Components.csproj</b><br/><small>net10.0</small>"]
        P2["<b>📦&nbsp;Atoms.Core.csproj</b><br/><small>net10.0</small>"]
        P3["<b>📦&nbsp;Atoms.UseCases.csproj</b><br/><small>net10.0</small>"]
        click P7 "../projects/Atoms.Components.md"
        click P2 "../projects/Atoms.Core.md"
        click P3 "../projects/Atoms.UseCases.md"
    end
    P1 --> MAIN
    P4 --> MAIN
    MAIN --> P7
    MAIN --> P2
    MAIN --> P3

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 9 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 5218 |  |
| ***Total APIs Analyzed*** | ***5227*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.10 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.SignalR.Client | 10.0.10 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.Design | 10.0.10 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.Tools | 10.0.10 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

