namespace Atoms.Core.Interfaces;

public interface IInviteSerializer
{
    string Serialize(Invite invite);
    Invite Deserialize(string code);
}
