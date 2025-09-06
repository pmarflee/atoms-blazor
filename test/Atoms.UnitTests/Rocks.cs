using Blazored.LocalStorage;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;

[assembly: Rock(typeof(IMediator), BuildType.Create)]
[assembly: Rock(typeof(IRandomNumberGenerator), BuildType.Create)]
[assembly: Rock(typeof(IDbContextFactory<>), BuildType.Create)]
[assembly: Rock(typeof(IPlayerStrategy), BuildType.Create)]
[assembly: Rock(typeof(IDataProtectionProvider), BuildType.Create)]
[assembly: Rock(typeof(IDataProtector), BuildType.Create)]
[assembly: Rock(typeof(IDateTimeService), BuildType.Create)]
[assembly: Rock(typeof(IInviteSerializer), BuildType.Create)]
[assembly: Rock(typeof(IValidator<>), BuildType.Create)]
[assembly: Rock(typeof(IBrowserStorageService), BuildType.Create)]
[assembly: Rock(typeof(IInviteSerializer), BuildType.Create)]
[assembly: Rock(typeof(ILocalStorageService), BuildType.Create)]
[assembly: Rock(typeof(IProtectedBrowserStorageService), BuildType.Create)]
[assembly: Rock(typeof(ILocalStorageUserService), BuildType.Create)]
