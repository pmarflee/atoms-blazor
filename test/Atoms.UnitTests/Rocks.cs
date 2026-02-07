using Blazored.LocalStorage;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

[assembly: Rock(typeof(IMediator), BuildType.Create)]
[assembly: Rock(typeof(IRandomNumberGenerator), BuildType.Create)]
[assembly: Rock(typeof(IDbContextFactory<>), BuildType.Create)]
[assembly: Rock(typeof(IPlayerStrategy), BuildType.Create)]
[assembly: Rock(typeof(IDataProtectionProvider), BuildType.Create)]
[assembly: Rock(typeof(IDataProtector), BuildType.Create)]
[assembly: Rock(typeof(IDateTimeService), BuildType.Create)]
[assembly: Rock(typeof(IValidator<>), BuildType.Create)]
[assembly: Rock(typeof(IBrowserStorageService), BuildType.Create)]
[assembly: Rock(typeof(ILocalStorageService), BuildType.Create)]
[assembly: Rock(typeof(IProtectedBrowserStorageService), BuildType.Create)]
[assembly: Rock(typeof(IVisitorService), BuildType.Create)]
[assembly: Rock(typeof(INotificationService), BuildType.Create)]
[assembly: Rock(typeof(IGameCreationService), BuildType.Create)]
[assembly: Rock(typeof(IBus), BuildType.Create)]
[assembly: Rock(typeof(ILogger<>), BuildType.Create)]
[assembly: Rock(typeof(IServiceScopeFactory), BuildType.Create)]
[assembly: Rock(typeof(IServiceProvider), BuildType.Create)]
[assembly: Rock(typeof(IServiceScope), BuildType.Create)]
