using Atoms.Infrastructure.Services;
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

    BrowserStorageService CreateBrowserStorageService() =>
        new(_localStorageServiceExpectations.Instance());
}
