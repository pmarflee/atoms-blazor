using System.Text;

namespace Atoms.Core.Entities;

public record UserIdentity(UserId? Id = null, string? Name = null)
{
    public UserIdentity(string? name) : this(null, name) { }

    public string? GetAbbreviatedName(IEnumerable<PlayerDTO>? players = null)
    {
        if (string.IsNullOrEmpty(Name)) return null;

        var words = Name.Split(' ', '_');
        var builder = new StringBuilder(words.Length);

        foreach (var word in words)
        {
            builder.Append(char.ToUpper(word[0]));
        }

        var abbreviatedName = builder.ToString();

        if (players is null || !players.Any()) return abbreviatedName;

        bool nameIsUnique;
        var tries = 0;
        string suffix;
        string abbreviatedNameWithSuffix;

        do
        {
            suffix = tries > 0 ? tries.ToString() : string.Empty;
            abbreviatedNameWithSuffix = abbreviatedName + suffix;
            nameIsUnique = players.All(p => p.AbbreviatedName
                                            != abbreviatedNameWithSuffix);
            tries++;
        }
        while (!nameIsUnique);

        return abbreviatedNameWithSuffix;
    }
}
