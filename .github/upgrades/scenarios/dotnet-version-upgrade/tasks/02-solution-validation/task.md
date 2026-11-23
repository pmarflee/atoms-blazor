# 02-solution-validation: Validate the complete upgraded solution

Run the complete unit-test suite and a final full solution build after the atomic upgrade. Verify that behavioral changes identified by the assessment—especially URI handling and the web application's authentication and middleware configuration—are covered by existing tests or focused regression tests. Confirm the Docker Compose project remains usable with the upgraded web project.

**Done when**: All tests pass, the complete solution builds without errors or warnings, and the .NET 11 behavior and Docker Compose integration have no unresolved regressions.

## Research findings

- The solution contains six SDK-style .NET projects, all now targeting `net11.0`, plus `docker-compose.dcproj`, which has no TFM. The validation scope is the full `Atoms.sln` and all 287 unit tests.
- The tests use TUnit and are not surfaced by Visual Studio Test Explorer; prior successful verification used `dotnet test` with `--output normal` (not VSTest's `--logger`).
- The upgrade assessment flags `System.Uri` behavioral changes and ASP.NET middleware/authentication changes. URI-sensitive call sites include `IdentityRedirectManager`, `NavigationExtensions`, `NotificationService`, and identity account pages; no focused unit tests for those call sites or the web authentication pipeline were found. `Program.cs` configures Identity cookies, confirmed-account registration, forwarded headers (non-development), response compression, exception handling/HSTS, antiforgery, and Razor components.
- `docker compose config --quiet` passes, and Compose builds `Atoms.Web` using the root `Dockerfile`. It still used ASP.NET runtime and SDK `10.0` images despite the web project targeting `net11.0`; additionally, its restore layer omitted the newly added root `Directory.Packages.props`. Update both image tags to `11.0` and copy the CPM file before restore, then validate the image build if Docker is available.
- No other source changes are currently indicated by the API/runtime review; perform full solution build and unit tests after the Dockerfile correction, and record any runtime limits explicitly.
