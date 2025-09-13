using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Atoms.Core.Data;
using System.Reflection;
using Atoms.Infrastructure.Data.DataProtection;
using Atoms.Core.Data.Identity;
using FluentValidation;

namespace Atoms.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddAtomsDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.BuildConnectionString();

        builder.Services.AddDbContext<ApplicationIdentityDbContext>(
            options => options.UseSqlite(connectionString),
            optionsLifetime: ServiceLifetime.Singleton);

        builder.Services.AddDbContextFactory<ApplicationIdentityDbContext>(
            options => options.UseSqlite(
                connectionString,
                x => x.MigrationsAssembly(Assembly.GetExecutingAssembly())));

        builder.Services.AddDbContextFactory<ApplicationDbContext>(
            options => options
                .UseSqlite(
                    connectionString,
                    x => x.MigrationsAssembly(Assembly.GetExecutingAssembly())));

        builder.Services.AddDbContext<DataProtectionKeyContext>(
            options => options.UseSqlite(connectionString));
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    static string BuildConnectionString(this WebApplicationBuilder builder)
    {
        return new SqliteConnectionStringBuilder
        {
            DataSource = Path.GetFullPath(
                Path.Combine(
                    builder.Environment.ContentRootPath, 
                    "database", "Atoms.db")),
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }
}
