using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

[assembly: Rock(typeof(IMediator), BuildType.Create)]
[assembly: Rock(typeof(IRandomNumberGenerator), BuildType.Create)]
[assembly: Rock(typeof(IDbContextFactory<>), BuildType.Create)]
[assembly: Rock(typeof(IPlayerStrategy), BuildType.Create)]
[assembly: Rock(typeof(IDataProtectionProvider), BuildType.Create)]
[assembly: Rock(typeof(IDataProtector), BuildType.Create)]
[assembly: Rock(typeof(IDateTimeService), BuildType.Create)]
[assembly: Rock(typeof(IInviteSerializer), BuildType.Create)]
[assembly: Rock(typeof(IValidator<>), BuildType.Create)]
