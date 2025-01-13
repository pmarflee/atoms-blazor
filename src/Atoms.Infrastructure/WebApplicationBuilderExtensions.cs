using Atoms.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atoms.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddAtomsDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationIdentityDbContext>(
            options => options.UseSqlite(BuildConnectionString(builder)));
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
