namespace Atoms.Core.Interfaces;

public interface IBrowserStorageService
{
    Task<StorageId> GetOrAddStorageId();
    Task<StorageId?> GetStorageId();
    ValueTask<string?> GetUserName();
    ValueTask SetUserName(string userName);
    ValueTask<ColourScheme> GetColourScheme();
    ValueTask SetColourScheme(ColourScheme colourScheme);
    ValueTask<AtomShape> GetAtomShape();
    ValueTask SetAtomShape(AtomShape atomShape);
}