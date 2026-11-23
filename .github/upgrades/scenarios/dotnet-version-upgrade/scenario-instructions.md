# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net11.0

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Project Structure
- Package Management: Central Package Management (CPM)

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

### Modernization
- Entity Framework: Keep EF6

## Strategy
**Selected**: All-at-Once
**Rationale**: Seven SDK-style projects are already on net10.0, with a three-level dependency graph and a small set of flagged breaking changes.

### Execution Constraints
- Upgrade all targetable projects in one atomic pass; do not sequence by dependency tier.
- Verify the SDK and global.json compatibility before project changes, and centralize package versions as a prerequisite.
- Update packages to .NET 11-compatible versions and remediate the vulnerabilities found in baseline restore.
- Restore and complete one bounded solution build/fix pass after project, package, and API updates; run tests after that pass.
- Keep EF6 in place during the framework upgrade; resolve the incompatible package and API changes inline.

## Decisions
- Recommended upgrade-option defaults are being used because interactive confirmation was unavailable.

## Source Control
- **Source Branch**: master
- **Working Branch**: upgrade-dotnet-11
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
