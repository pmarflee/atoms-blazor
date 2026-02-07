using Atoms.Core;
using Atoms.Core.Data.Identity;
using Atoms.Core.Delegates;
using Atoms.Core.Entities.Configuration;
using Atoms.Infrastructure;
using Atoms.Infrastructure.Data.DataProtection;
using Atoms.Infrastructure.Email;
using Atoms.Infrastructure.Factories;
using Atoms.Infrastructure.Middleware;
using Atoms.Infrastructure.Services;
using Atoms.Infrastructure.SignalR;
using Atoms.UseCases.CreateNewGame;
using Atoms.UseCases.PlayerMove.Rebus;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.FluentUI.AspNetCore.Components;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddFluentUIComponents();
    builder.Services.AddDataGridEntityFrameworkAdapter();

    builder.Services.AddSingleton<CreateRng>(RngFactory.Create);
    builder.Services.AddSingleton<CreatePlayerStrategy>(PlayerStrategyFactory.Create);
    builder.Services.AddScoped<CreateGame>(sp =>
    {
        var rngFactory = sp.GetRequiredService<CreateRng>();
        var dateTimeService = sp.GetRequiredService<IDateTimeService>();

        return (options, visitorId, userIdentity, gameId) =>
            GameFactory.Create(rngFactory, dateTimeService,
                               options, visitorId, userIdentity, gameId);
    });
    builder.Services.AddScoped<GameStateContainer>();

    builder.Services
        .AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateNewGameRequest>();
            cfg.LicenseKey = builder.Configuration["MediatR:LicenceKey"];
        });

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

    var atomsDbConnectionString = builder.Configuration.GetConnectionString("AtomsDb")
        ?? throw new Exception("Atoms Db connection string is null");


    builder.AddAtomsDatabase(atomsDbConnectionString);

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

    builder.Services.AddBlazoredLocalStorage();
    builder.Services.AddScoped<IBrowserStorageService, BrowserStorageService>();
    builder.Services.AddScoped<IProtectedBrowserStorageService, ProtectedBrowserStorageService>();
    builder.Services.AddScoped<IVisitorService, VisitorService>();
    builder.Services.AddSingleton<VisitorIdCookieValueService>();

    builder.AddValidation();

    builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
    builder.Services.AddSingleton<IGameMoveService, GameMoveService>();

    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddSignalR();

    builder.Services.AddScoped<IGameCreationService, GameCreationService>();

    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            ["application/octet-stream"]);
    });

    builder.Services.AddRebus(
        configure => configure
            .Transport(
                t =>
                {
                    const string queueName = "message-queue";

                    if (builder.Environment.IsDevelopment())
                    {
                        t.UseInMemoryTransport(new InMemNetwork(),
                                               queueName);
                    }
                    else
                    {
                        t.UsePostgreSql(atomsDbConnectionString,
                                        "Rebus_Messages",
                                        queueName);
                    }
                })
            .Routing(
                r => r.TypeBased()
                    .MapAssemblyOf<PlayerMoveMessage>("message-queue"))
        );

    builder.Services.AutoRegisterHandlersFromAssemblyOf<PlayerMoveMessageHandler>();

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.Configure<AppSettings>(
        builder.Configuration.GetSection("AppSettings"));

    builder.Services.AddCascadingValue(sp =>
    {
        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext!;
        var visitorIdCookieValueService = sp.GetRequiredService<VisitorIdCookieValueService>();

        return visitorIdCookieValueService.TryGetCookieValue(
            httpContext, out var visitorIdCookieValue)
            ? visitorIdCookieValue.Id
            : VisitorId.Empty;
    });

    builder.Services.AddCascadingValue(
        Constants.CascadingValues.VisitorUserName,
        sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext!;
            var visitorIdCookieValueService = sp.GetRequiredService<VisitorIdCookieValueService>();

            return visitorIdCookieValueService.TryGetCookieValue(
                httpContext, out var visitorIdCookieValue)
                ? visitorIdCookieValue.Name
                : null;
        });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (!app.Environment.IsDevelopment())
    {
        app.UseResponseCompression();
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    //app.UseHttpsRedirection();

    app.MapGet("/music/{filename}", ([FromRoute] string filename) =>
    {
        var path = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "audio", filename);

        return Results.File(path, "audio/mpeg");
    });

    app.MapStaticAssets();
    app.UseAntiforgery();

    app.UseMiddleware<SetVisitorIdCookie>();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.MapAdditionalIdentityEndpoints();

    app.MapHub<GameHub>(GameHub.HubUrl);

    app.RunDatabaseMigrations();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
