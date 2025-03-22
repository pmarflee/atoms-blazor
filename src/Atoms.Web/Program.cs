using Atoms.Core.Data.Identity;
using Atoms.Core.Delegates;
using Atoms.Core.Entities.Configuration;
using Atoms.Core.Interfaces;
using Atoms.Core.Serialization;
using Atoms.Core.Services;
using Atoms.Infrastructure;
using Atoms.Infrastructure.Data.DataProtection;
using Atoms.Infrastructure.Email;
using Atoms.Infrastructure.Factories;
using Atoms.Infrastructure.Validation;
using Atoms.UseCases.CreateNewGame;
using FluentValidation;
using MediatR.Courier;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NReco.Logging.File;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<CreateRng>(RngFactory.Create);
builder.Services.AddSingleton<CreatePlayerStrategy>(PlayerStrategyFactory.Create);
builder.Services.AddSingleton<CreateGame>(sp =>
{
    var rngFactory = sp.GetRequiredService<CreateRng>();
    var playerStrategyFactory = sp.GetRequiredService<CreatePlayerStrategy>();

    return options => GameFactory.Create(rngFactory,
                                         playerStrategyFactory,
                                         options);
});

builder.Services.AddScoped<GameStateContainer>();

builder.Services.AddScoped<IInviteSerializer, InviteSerializer>();

builder.Services
    .AddMediatR(cfg =>
        cfg.RegisterServicesFromAssemblyContaining<CreateNewGameRequest>())
    .AddCourier(typeof(CreateNewGameRequest).Assembly);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.AddAtomsDatabase();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddIdentityCore<ApplicationUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>();
builder.Services.AddSingleton<IEmailSender, MailgunApiEmailSender>();

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeyContext>()
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

builder.Services.AddOptions<EmailSettings>()
                .BindConfiguration("Email");

builder.Services.AddScoped<IBrowserStorageService, BrowserStorageService>();

builder.Services.AddTransient<IValidator<Invite>, InviteValidator>();

builder.Services.AddSingleton<IDateTimeService, DateTimeService>();

var loggingConfigurationSection = builder.Configuration.GetSection("Logging");

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddFile(loggingConfigurationSection,
        fileLoggerOpts =>
        {
            fileLoggerOpts.FormatLogFileName =
                fName => string.Format(fName, DateTime.UtcNow);

            var logFileRetentionDays = loggingConfigurationSection
                .GetValue<int>("File:RetentionDays");
            var lastRetentionDate = DateTime.UtcNow.Date.Subtract(
                TimeSpan.FromDays(logFileRetentionDays));

            foreach (var filename in Directory.GetFiles("logs", "*.log"))
            {
                var file = new FileInfo(filename);

                if (file.CreationTimeUtc.Date < lastRetentionDate)
                {
                    file.Delete();
                }
            }
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapGet("/music/{filename}", ([FromRoute] string filename) =>
{
    var path = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "audio", filename);

    return Results.File(path, "audio/mpeg");
});

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.RunDatabaseMigrations();

app.Run();
