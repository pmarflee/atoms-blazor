using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Atoms.Core.Entities;

public record UserIdentity(UserId? Id = null, string? Name = null)
{
    public UserIdentity(string? name) : this(null, name) { }

    public string? GetAbbreviatedName(IEnumerable<PlayerDTO>? players = null)
    {
        if (string.IsNullOrEmpty(Name)) return null;

        var words = Name.Split(' ', '_', '-');

        static char GetFirstLetter(string word) => char.ToUpper(word[0]);

        var firstChars = words.Select(GetFirstLetter);

        if (players is null || !players.Any())
        {
            return new string([.. firstChars]);
        }

        var playerAbbreviatedChars = players
            .Where(p => p.PlayerTypeId == PlayerType.Human
                        && !string.IsNullOrEmpty(p.AbbreviatedName))
            .Select(p => p.AbbreviatedName!.ToArray())
            .ToList();
         var abbreviatedChars = new LinkedList<char>(firstChars);
        var insertAt = abbreviatedChars.First!;
        var wordIndex = 0;
        var letterIndex = 1;

        bool NameIsUnique() => playerAbbreviatedChars.All(
            p => p?.SequenceEqual(abbreviatedChars) == false);

        while (wordIndex < words.Length && !NameIsUnique())
        {
            if (letterIndex >= words[wordIndex].Length)
            {
                wordIndex++;
                letterIndex = 1;
                insertAt = insertAt!.Next;
            }
            else
            {
                insertAt = abbreviatedChars.AddAfter(
                    insertAt!,
                    words[wordIndex][letterIndex]);

                letterIndex++;
            }
        }

        var i = 1;

        if (wordIndex == words.Length)
        {
            do
            {
                if (i > 1)
                {
                    abbreviatedChars.RemoveLast();
                }

                var charDigit = (char)(i++ + '0');

                abbreviatedChars.AddAfter(abbreviatedChars.Last!, charDigit);
            }
            while (!NameIsUnique());
        }

        return new string([.. abbreviatedChars]);
    }
}
