using Atoms.Core.AI.Strategies;
using Atoms.Core.Interfaces;
using Atoms.Core.Services;
using Atoms.UseCases.CreateNewGame;
using MediatR.Courier;

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

builder.Services
    .AddMediatR(cfg => 
        cfg.RegisterServicesFromAssemblyContaining<CreateNewGameRequest>())
    .AddCourier(typeof(CreateNewGameRequest).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
