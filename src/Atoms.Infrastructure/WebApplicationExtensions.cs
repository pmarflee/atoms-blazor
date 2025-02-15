using Atoms.Core.Data;
using Atoms.Infrastructure.Data.DataProtection;
using Atoms.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Atoms.Infrastructure;

public static class WebApplicationExtensions
{
    public static void RunDatabaseMigrations(this WebApplication app)
    {
        RunIdentityMigrations(app);
        RunApplicationMigrations(app);
        RunDataProtectionMigrations(app);
    }

    static void RunIdentityMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        db.Database.Migrate();
    }

    static void RunApplicationMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Database.Migrate();
    }

    static void RunDataProtectionMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<DataProtectionKeyContext>();

        db.Database.Migrate();
    }
}
