# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v11.0.

Detailed findings live alongside this file in `assessment/`. This page is the index: read it first, then open only the documents you need.

## Table of Contents

- [Executive Summary](#executive-summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Detailed Reports](#detailed-reports)
  - [Projects Relationship Graph](assessment/project-graph.md)
  - [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
  - [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)
  - [Project Details](#project-details)

## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 7 | 6 require upgrade |
| Total NuGet Packages | 144 | 11 need upgrade |
| Total Code Files | 216 |  |
| Total Code Files with Incidents | 19 |  |
| Total Lines of Code | 12874 |  |
| Total Number of Issues | 65 |  |
| Proposed Target Framework | net11.0 |  |
| Estimated LOC to modify | 42+ | at least 0.3% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [docker-compose.dcproj](assessment/projects/docker-compose.md) |  | ✅ None | 0 | 0 | 0 |  | DotNetCoreApp, Sdk Style = True |
| [src\Atoms.Components\Atoms.Components.csproj](assessment/projects/Atoms.Components.md) | net10.0 | 🟢 Low | 1 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src\Atoms.Core\Atoms.Core.csproj](assessment/projects/Atoms.Core.md) | net10.0 | 🟢 Low | 3 | 10 | 0 | 10+ | ClassLibrary, Sdk Style = True |
| [src\Atoms.Infrastructure\Atoms.Infrastructure.csproj](assessment/projects/Atoms.Infrastructure.md) | net10.0 | 🟢 Low | 4 | 9 | 0 | 9+ | ClassLibrary, Sdk Style = True |
| [src\Atoms.UseCases\Atoms.UseCases.csproj](assessment/projects/Atoms.UseCases.md) | net10.0 | 🟢 Low | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src\Atoms.Web\Atoms.Web.csproj](assessment/projects/Atoms.Web.md) | net10.0 | 🟢 Low | 6 | 23 | 0 | 23+ | AspNetCore, Sdk Style = True |
| [test\Atoms.UnitTests\Atoms.UnitTests.csproj](assessment/projects/Atoms.UnitTests.md) | net10.0 | 🟢 Low | 3 | 0 | 0 |  | DotNetCoreApp, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 133 | 92.4% |
| ⚠️ Incompatible | 1 | 0.7% |
| 🔄 Upgrade Recommended | 10 | 6.9% |
| ***Total NuGet Packages*** | ***144*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 1 | High - Require code changes |
| 🟡 Source Incompatible | 9 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 32 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 23488 |  |
| ***Total APIs Analyzed*** | ***23530*** |  |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:System.Uri | 17 | 40.5% | Behavioral Change |
| P:System.Uri.AbsoluteUri | 7 | 16.7% | Behavioral Change |
| M:System.String.Join(System.String,System.ReadOnlySpan{System.String}) | 3 | 7.1% | Source Incompatible |
| M:System.Uri.#ctor(System.String) | 2 | 4.8% | Behavioral Change |
| P:Microsoft.AspNetCore.Identity.SignInOptions.RequireConfirmedAccount | 2 | 4.8% | Behavioral Change |
| M:Microsoft.AspNetCore.Builder.ExceptionHandlerExtensions.UseExceptionHandler(Microsoft.AspNetCore.Builder.IApplicationBuilder,System.String,System.Boolean) | 1 | 2.4% | Behavioral Change |
| M:Microsoft.AspNetCore.Builder.ForwardedHeadersExtensions.UseForwardedHeaders(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Builder.ForwardedHeadersOptions) | 1 | 2.4% | Behavioral Change |
| M:Microsoft.AspNetCore.Builder.ResponseCompressionBuilderExtensions.UseResponseCompression(Microsoft.AspNetCore.Builder.IApplicationBuilder) | 1 | 2.4% | Behavioral Change |
| M:Microsoft.Extensions.DependencyInjection.DatabaseDeveloperPageExceptionFilterServiceExtensions.AddDatabaseDeveloperPageExceptionFilter(Microsoft.Extensions.DependencyInjection.IServiceCollection) | 1 | 2.4% | Source Incompatible |
| M:Microsoft.Extensions.DependencyInjection.IdentityEntityFrameworkBuilderExtensions.AddEntityFrameworkStores''1(Microsoft.AspNetCore.Identity.IdentityBuilder) | 1 | 2.4% | Source Incompatible |

The table above is the top 10. See [the complete list](assessment/api-issues/most-frequent-api-issues.md) for every affected API.

## Detailed Reports

- [Projects Relationship Graph](assessment/project-graph.md)
- [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
- [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)

### Project Details

- [docker-compose.dcproj](assessment/projects/docker-compose.md)
- [src\Atoms.Components\Atoms.Components.csproj](assessment/projects/Atoms.Components.md)
- [src\Atoms.Core\Atoms.Core.csproj](assessment/projects/Atoms.Core.md)
- [src\Atoms.Infrastructure\Atoms.Infrastructure.csproj](assessment/projects/Atoms.Infrastructure.md)
- [src\Atoms.UseCases\Atoms.UseCases.csproj](assessment/projects/Atoms.UseCases.md)
- [src\Atoms.Web\Atoms.Web.csproj](assessment/projects/Atoms.Web.md)
- [test\Atoms.UnitTests\Atoms.UnitTests.csproj](assessment/projects/Atoms.UnitTests.md)


