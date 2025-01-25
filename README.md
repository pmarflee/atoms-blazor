# Atoms-Blazor

An ASP.NET / Blazor implemention of the Amiga Atoms game.

Based on the original web implementation by Thomas Pike: https://github.com/thomas-pike/atoms-www

Play online at https://atoms-blazor.salmonplant-c5991439.uksouth.azurecontainerapps.io

dotnet ef migrations add AddGame --project ..\atoms.infrastructure\atoms.infrastructure.csproj --context ApplicationDbContext --namespace Atoms.Infrastructure.Data.Migrations -o ..\Atoms.Infrastructure\Data\Migrations