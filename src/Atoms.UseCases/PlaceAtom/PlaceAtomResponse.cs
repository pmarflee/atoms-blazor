namespace Atoms.UseCases.PlaceAtom;

public class PlaceAtomResponse
{
    private PlaceAtomResponse(bool isSuccessful)
    {
        IsSuccessful = isSuccessful;
    }

    public bool IsSuccessful { get; }

    public static PlaceAtomResponse Success = new(true);
    public static PlaceAtomResponse Failure = new(false);
}
