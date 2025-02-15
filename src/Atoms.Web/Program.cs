using Atoms.Core.AI.Strategies;
using Atoms.Core.Entities.Configuration;
using Atoms.Core.Factories;
using Atoms.Core.Interfaces;
using Atoms.Core.Services;
using Atoms.Infrastructure;
using Atoms.Infrastructure.Data.Identity;
using Atoms.Infrastructure.Email;
using Atoms.UseCases.CreateNewGame;
using MediatR.Courier;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<Func<int, int, IRandomNumberGenerator>>(sp =>
{
    return (seed, iterations) =>
    {
        var random = new Random(seed);
        var i = 0;

        while (i++ < iterations)
        {
            random.Next();
        }

        return new RandomNumberGenerator(random, seed, iterations);
    };
});

builder.Services.AddSingleton<Func<PlayerType, IRandomNumberGenerator, IPlayerStrategy?>>(sp =>
{
    return (playerType, rng) => playerType switch
    {
        PlayerType.CPU_Easy => new PlayRandomly(rng),
        PlayerType.CPU_Medium =>
            new FullyLoadedCellNextToEnemyFullyLoadedCell()
            .Or(new PlaySemiRandomlyAvoidingDangerCells(rng)),
        PlayerType.CPU_Hard =>
            new FullyLoadedCellNextToEnemyFullyLoadedCell()
                .Or(new GainAdvantageOverNeighbouringCell(rng))
                .Or(new ChooseCornerCell(rng))
                .Or(new PlaySemiRandomlyAvoidingDangerCells(rng)),
        _ => null
    };
});

builder.Services.AddSingleton<Func<GameMenuOptions, Game>>(sp =>
{
    return options =>
    {
        var rngFactory = sp.GetRequiredService<Func<int, int, IRandomNumberGenerator>>();
        var playerStrategyFactory = sp.GetRequiredService<Func<PlayerType, IRandomNumberGenerator, IPlayerStrategy>>();

        return GameFactory.Create(rngFactory,
                                  playerStrategyFactory,
                                  Guid.NewGuid(),
                                  options);
    };
});

builder.Services.AddScoped<GameStateContainer>();

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

builder.Services.AddOptions<EmailSettings>()
                .BindConfiguration("Email");

builder.Services.AddScoped<BrowserStorageService>();

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
