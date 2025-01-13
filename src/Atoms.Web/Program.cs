using Atoms.Core.AI.Strategies;
using Atoms.Core.Entities.Configuration;
using Atoms.Core.Identity;
using Atoms.Core.Interfaces;
using Atoms.Core.Services;
using Atoms.Infrastructure;
using Atoms.Infrastructure.Data.Identity;
using Atoms.Infrastructure.Email;
using Atoms.Infrastructure.Identity;
using Atoms.UseCases.CreateNewGame;
using MediatR.Courier;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<Func<PlayerType, IPlayerStrategy?>>(sp =>
{
    var rng = new RandomNumberGenerator(new Random());

    return playerType => playerType switch
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapGet("/music/{filename}", ([FromRoute]string filename) =>
{
    var path = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "audio", filename);

    return Results.File(path, "audio/mpeg");
});

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

    db.Database.Migrate();
}

app.Run();
