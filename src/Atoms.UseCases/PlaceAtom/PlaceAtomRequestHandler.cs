namespace Atoms.UseCases.PlaceAtom;

public class PlaceAtomRequestHandler(IMediator mediator) 
    : IRequestHandler<PlaceAtomRequest, PlaceAtomResponse>
{
    public Task<PlaceAtomResponse> Handle(PlaceAtomRequest request,
                       CancellationToken cancellationToken)
    {
        var game = request.Game;
        var cell = request.Cell;

        if (!game.CanPlaceAtom(cell))
        {
            return Task.FromResult(PlaceAtomResponse.Failure);
        }

        return Task.FromResult(PlaceAtomResponse.Success);
    }
}
