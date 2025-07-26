using Atoms.Core;
using Atoms.Core.Services;
using Blazored.LocalStorage;

namespace Atoms.UnitTests.Core.Services;

public class BrowserStorageServiceTests
{
    ILocalStorageServiceCreateExpectations _localStorageServiceExpectations = default!;

    [Before(Test)]
    public Task Setup()
    {
        _localStorageServiceExpectations = new ILocalStorageServiceCreateExpectations();

        return Task.CompletedTask;
    }

    [Test]
    public async Task GetOrAddStorageIdShouldReturnCurrentValueIfValueIsStoredInLocalStorage()
    {
        _localStorageServiceExpectations.Methods
            .GetItemAsync<Guid?>(Arg.Is(Constants.StorageKeys.LocalStorageId))
            .ReturnValue(ValueTask.FromResult<Guid?>(ObjectMother.LocalStorageId.Value));

        var service = CreateBrowserStorageService();

        await Assert.That(await service.GetOrAddStorageId())
            .IsEqualTo(ObjectMother.LocalStorageId);
    }

    [Test]
    public async Task GetOrAddStorageIdShouldGenerateANewValueIfValueIsCurrentlyNotStoredInLocalStorage()
    {
        _localStorageServiceExpectations.Methods
            .GetItemAsync<Guid?>(Arg.Is(Constants.StorageKeys.LocalStorageId))
            .ReturnValue(ValueTask.FromResult<Guid?>(null));

        _localStorageServiceExpectations.Methods
            .SetItemAsync(
                Arg.Is(Constants.StorageKeys.LocalStorageId),
                Arg.Is(ObjectMother.LocalStorageId.Value));

        var service = CreateBrowserStorageService();

        await Assert.That(await service.GetOrAddStorageId())
            .IsEqualTo(ObjectMother.LocalStorageId);

        _localStorageServiceExpectations.Verify();
    }

    BrowserStorageService CreateBrowserStorageService() =>
        new(_localStorageServiceExpectations.Instance(),
            () => ObjectMother.LocalStorageId.Value);
}
