# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the solution to .NET 11 (`net11.0`).
**Scope**: Seven SDK-style projects; six currently target `net10.0`, and the solution also contains a Docker Compose project.

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: Seven projects are SDK-style and on .NET 10, with a clear three-level dependency structure and only one incompatible package.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | Seven SDK-style projects are already on net10.0, with a three-level dependency graph and one incompatible package. |
| Package Management | Central Package Management (CPM) | Seven SDK-style projects have no existing `Directory.Packages.props`, so centralizing package versions supports consistent updates. |
| Unsupported Packages | Resolve Inline | The assessment identifies one incompatible package in `Atoms.Web`, small enough to resolve as part of this upgrade. |
| Unsupported API Handling | Fix Inline | The assessment flags 1 binary and 9 source-incompatible APIs, without a large-scale migration. |
| Entity Framework | Keep EF6 | The solution uses EF6 6.5.2; upgrading .NET first avoids combining framework and EF Core migration changes. |

## Tasks

### 01-central-package-management: Centralize packages and upgrade the solution to .NET 11

Verify the installed SDK and `global.json` support `net11.0`, then use Central Package Management while upgrading all six framework-targeted projects in one atomic pass: Atoms.Components, Atoms.Core, Atoms.Infrastructure, Atoms.UseCases, Atoms.Web, and Atoms.UnitTests. The solution has seven SDK-style projects and no existing `Directory.Packages.props`. Preserve package metadata such as `PrivateAssets` and `IncludeAssets`; the reported `System.Security.Cryptography.Pkcs` version split is transitive, not a direct-reference conflict, so do not add a transitive pin or `VersionOverride` without evidence it is needed.

Retarget the six projects to `net11.0`, update packages to compatible versions, address the assessment's one incompatible package and the source/binary-incompatible APIs, and account for behavioral changes in URI handling and ASP.NET Core. Baseline restore also revealed advisories affecting `Microsoft.AspNetCore.SignalR.Common` (GHSA-334f-vvp4-7g6p), `System.Security.Cryptography.Pkcs` (GHSA-2fmv-rv2c-59x7 and GHSA-846f-f764-76mx), and `System.Security.Cryptography.Xml` (GHSA-7hqp-7c5m-4vq6, GHSA-c399-9853-6jmj, and GHSA-r955-8q5j-3h83); resolve them using supported patched package versions. Keep Entity Framework 6.5.2 in place and preserve the Docker Compose project without adding a TFM.

**Done when**: All six .NET projects target `net11.0`; direct package versions are centrally managed; the incompatible package and breaking APIs are resolved; the identified vulnerability warnings are gone; and the full solution builds without errors or warnings.

### 02-solution-validation: Validate the complete upgraded solution

Run the complete unit-test suite and a final full solution build after the atomic upgrade. Verify that behavioral changes identified by the assessment—especially URI handling and the web application's authentication and middleware configuration—are covered by existing tests or focused regression tests. Confirm the Docker Compose project remains usable with the upgraded web project.

**Done when**: All tests pass, the complete solution builds without errors or warnings, and the .NET 11 behavior and Docker Compose integration have no unresolved regressions.
