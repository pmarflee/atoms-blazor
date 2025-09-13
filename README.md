# Atoms-Blazor

An ASP.NET / Blazor implemention of the Amiga Atoms game.

Based on the original web implementation by Thomas Pike: https://github.com/thomas-pike/atoms-www

Play online at https://atoms-blazor-9xylq.ondigitalocean.app/

Migration commands:

```
dotnet ef migrations add AddGame --project ..\atoms.infrastructure\atoms.infrastructure.csproj --context ApplicationDbContext --namespace Atoms.Infrastructure.Data.Migrations -o ..\Atoms.Infrastructure\Data\Migrations
dotnet ef migrations add AddDataProtectionKeys --project ..\atoms.infrastructure\atoms.infrastructure.csproj --context DataProtectionKeyContext --namespace Atoms.Infrastructure.Data.DataProtection.Migrations -o ..\Atoms.Infrastructure\Data\DataProtection\Migrations
dotnet tool update --global dotnet-ef
```

How to authorize with doctl:

```
doctl auth init --context <my-context>
doctl registry login
```

How to build and push a Docker image to the DigitalOcean container registry:
* Switch to the root folder in the git repository
* Run the following commands:
```
docker compose build
docker tag atomsweb <image-name>
docker push <image-name>
```