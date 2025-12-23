using Atoms.Core;
using Atoms.Infrastructure.Services;
using Blazored.LocalStorage;

namespace Atoms.UnitTests.Core.Services;

public class BrowserStorageServiceTests
{
    ILocalStorageServiceCreateExpectations _localStorageServiceExpectations = default!;
    IProtectedBrowserStorageServiceCreateExpectations _protectedBrowserStorageService = default!;

    [Before(Test)]
    public Task Setup()
    {
        _localStorageServiceExpectations = new ILocalStorageServiceCreateExpectations();
        _protectedBrowserStorageService = new IProtectedBrowserStorageServiceCreateExpectations();

        return Task.CompletedTask;
    }

    [Test]
    public async Task GetOrAddStorageIdShouldReturnCurrentValueIfValueIsStoredInLocalStorage()
    {
        _protectedBrowserStorageService.Setups
            .GetAsync<Guid?>(Arg.Is(Constants.StorageKeys.LocalStorageId))
            .ReturnValue(ValueTask.FromResult<Guid?>(ObjectMother.LocalStorageId.Value));

        var service = CreateBrowserStorageService();

        await Assert.That(await service.GetOrAddStorageId())
            .IsEqualTo(ObjectMother.LocalStorageId);
    }

    [Test]
    public async Task GetOrAddStorageIdShouldGenerateANewValueIfValueIsCurrentlyNotStoredInLocalStorage()
    {
        _protectedBrowserStorageService.Setups
            .GetAsync<Guid?>(Arg.Is(Constants.StorageKeys.LocalStorageId))
            .ReturnValue(ValueTask.FromResult<Guid?>(null));

        _protectedBrowserStorageService.Setups
            .SetAsync(
                Arg.Is(Constants.StorageKeys.LocalStorageId),
                Arg.Is<object>(ObjectMother.LocalStorageId.Value))
            .ReturnValue(ValueTask.CompletedTask);

        var service = CreateBrowserStorageService();

        await Assert.That(await service.GetOrAddStorageId())
            .IsEqualTo(ObjectMother.LocalStorageId);

        _localStorageServiceExpectations.Verify();
    }

    BrowserStorageService CreateBrowserStorageService() =>
        new(_localStorageServiceExpectations.Instance(),
            _protectedBrowserStorageService.Instance(),
            () => ObjectMother.LocalStorageId.Value);
}
