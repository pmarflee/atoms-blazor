namespace Atoms.Web.CustomEvents;

public class CellClickEventArgs(Position position) : EventArgs
{
    public Position Position { get; } = position;
}
