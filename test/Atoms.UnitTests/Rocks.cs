using Microsoft.AspNetCore.DataProtection;

[assembly: Rock(typeof(IMediator), BuildType.Create)]
[assembly: Rock(typeof(IRandomNumberGenerator), BuildType.Create)]
[assembly: Rock(typeof(IDbContextFactory<>), BuildType.Create)]
[assembly: Rock(typeof(IPlayerStrategy), BuildType.Create)]
[assembly: Rock(typeof(IDataProtectionProvider), BuildType.Create)]
[assembly: Rock(typeof(IDataProtector), BuildType.Create)]
