# Projects Relationship Graph

[← Back to the assessment index](../assessment.md)

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;Atoms.Web.csproj</b><br/><small>net10.0</small>"]
    P2["<b>📦&nbsp;Atoms.Core.csproj</b><br/><small>net10.0</small>"]
    P3["<b>📦&nbsp;Atoms.UseCases.csproj</b><br/><small>net10.0</small>"]
    P4["<b>📦&nbsp;Atoms.UnitTests.csproj</b><br/><small>net10.0</small>"]
    P5["<b>📦&nbsp;Atoms.Infrastructure.csproj</b><br/><small>net10.0</small>"]
    P6["<b>📦&nbsp;docker-compose.dcproj</b><br/><small></small>"]
    P7["<b>📦&nbsp;Atoms.Components.csproj</b><br/><small>net10.0</small>"]
    P1 --> P5
    P1 --> P2
    P1 --> P3
    P3 --> P2
    P4 --> P3
    P4 --> P2
    P4 --> P5
    P5 --> P7
    P5 --> P2
    P5 --> P3
    click P1 "projects/Atoms.Web.md"
    click P2 "projects/Atoms.Core.md"
    click P3 "projects/Atoms.UseCases.md"
    click P4 "projects/Atoms.UnitTests.md"
    click P5 "projects/Atoms.Infrastructure.md"
    click P6 "projects/docker-compose.md"
    click P7 "projects/Atoms.Components.md"

```

