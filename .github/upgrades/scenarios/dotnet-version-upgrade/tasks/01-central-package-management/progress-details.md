# Task 01 Progress

- Added root `Directory.Packages.props` with Central Package Management enabled only for the six .NET projects in `Atoms.sln`; removed inline package versions from those project files while preserving package metadata.
- Retargeted Atoms.Components, Atoms.Core, Atoms.Infrastructure, Atoms.UseCases, Atoms.Web, and Atoms.UnitTests from `net10.0` to `net11.0`.
- Updated ASP.NET Core, EF Core, SQLite, and Npgsql EF Core direct dependencies to `11.0.0`; updated TUnit from `1.63.0` to `1.87.12` to run under Microsoft Testing Platform with the .NET 11 SDK.
- Removed the unsupported `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` reference (1.23.0 is a .NET Framework 4.7.2 tooling package with no stable .NET 11-compatible version). The Docker Compose project remains in the solution.
- Kept the out-of-solution `src/Atoms.EmailTemplates` project outside the TFM and CPM scope. Its `net10.0` restore succeeds with its existing package version.
- `dotnet build Atoms.sln`: succeeded, 0 warnings, 0 errors. Visual Studio MSBuild solution build: succeeded.
- `dotnet test test\Atoms.UnitTests\Atoms.UnitTests.csproj --output normal`: 287 passed, 0 failed.
- Vulnerability audits with transitive packages enabled reported no vulnerable packages for any of the six .NET projects.
- An initial VSTest `--logger` argument was rejected by this Microsoft Testing Platform project; rerunning with the supported `--output normal` option passed.
