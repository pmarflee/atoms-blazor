using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Atoms.Infrastructure.Data.Identity;
using Atoms.Core.Data;
using System.Reflection;
using Atoms.Infrastructure.Data.DataProtection;

namespace Atoms.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddAtomsDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = BuildConnectionString(builder);

        builder.Services.AddDbContext<ApplicationIdentityDbContext>(
            options => options.UseSqlite(connectionString));

        builder.Services.AddDbContextFactory<ApplicationDbContext>(
            options => options.UseSqlite(
                connectionString,
                x => x.MigrationsAssembly(Assembly.GetExecutingAssembly())));

        builder.Services.AddDbContext<DataProtectionKeyContext>(
            options => options.UseSqlite(connectionString));
    }

    static string BuildConnectionString(WebApplicationBuilder builder)
    {
        return new SqliteConnectionStringBuilder
        {
            DataSource = Path.GetFullPath(
            Path.Combine(
                builder.Environment.ContentRootPath,
                "Database",
                "Atoms.db")),

            Cache = SqliteCacheMode.Shared
        }.ToString();
    }
}
