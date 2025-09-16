using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Atoms.Core.Data;
using System.Reflection;
using Atoms.Infrastructure.Data.DataProtection;
using Atoms.Core.Data.Identity;
using FluentValidation;

namespace Atoms.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddAtomsDatabase(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddDbContext<ApplicationIdentityDbContext>(
            options => options.UseNpgsql(connectionString),
            optionsLifetime: ServiceLifetime.Singleton);

        builder.Services.AddDbContextFactory<ApplicationIdentityDbContext>(
            options => options.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly(Assembly.GetExecutingAssembly())));

        builder.Services.AddDbContextFactory<ApplicationDbContext>(
            options => options.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly(Assembly.GetExecutingAssembly())));

        builder.Services.AddDbContext<DataProtectionKeyContext>(
            options => options.UseNpgsql(connectionString));
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
