namespace Atoms.Web.Components;

public abstract class Component2Base : ComponentBase
{
    [Inject]
    public IMediator Mediator { get; set; } = default!;
}
