# Atoms-Blazor

<div style="text-align: center; margin-bottom: 15px;">
    <img src="https://atoms-blazor.app/images/screenshot.png" alt="Game board" title="Game board" height="400">
</div>

An ASP.NET / Blazor implemention of the Amiga Atoms game.

Based on the original web implementation by Thomas Pike: https://github.com/thomas-pike/atoms-www

Play online at https://atoms-blazor.app/

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

How to set up Postgres database user:
```
-- Step 1: Create database user
CREATE USER atoms_user_ WITH PASSWORD 'your_strong_password';

-- Step 2: Revoke existing privileges from the public role
REVOKE ALL ON SCHEMA public FROM public;

-- Step 3: Grant the new user ownership of the public schema
-- This is a very powerful permission and should be used with care
ALTER SCHEMA public OWNER TO atoms_user;

-- Step 4: Grant all privileges on all existing tables in the public schema to the new user
-- This handles any pre-existing tables, like __EFMigrationsHistory, that might have been created by doadmin
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO atoms_user;

-- Step 5: Set default privileges for all future tables to be created by the new user
ALTER DEFAULT PRIVILEGES FOR USER atoms_user IN SCHEMA public GRANT ALL ON TABLES TO atoms_user;

-- Step 6: Grant all privileges on the database to the new user
GRANT ALL PRIVILEGES ON DATABASE "Atoms" TO atoms_user;

-- Step 7: (Optional but recommended) Re-grant CONNECT to the public role for other users
-- This allows other users (if any) to connect without affecting your schema
GRANT CONNECT ON DATABASE "Atoms" TO public;
```