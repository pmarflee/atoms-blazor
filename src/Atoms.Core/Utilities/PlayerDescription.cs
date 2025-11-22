using System.Text;

namespace Atoms.Core.Utilities;

public static class PlayerDescription
{
    public static string Build(int number, PlayerType type, string? name)
    {
        var builder = new StringBuilder($"Player {number}");

        if (type == PlayerType.Human)
        {
            if (!string.IsNullOrEmpty(name))
            {
                builder.Append($" ({name})");
            }
        }
        else
        {
            builder.Append($" ({type.Description})");
        }

        return builder.ToString();
    }
}
