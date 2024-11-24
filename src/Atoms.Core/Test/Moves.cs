using System.Collections;

namespace Atoms.Core.Test;

public class Moves : IEnumerable<(int, int)>
{
    static List<(int, int)> Player1 =>
        [
            (4, 4), (1, 9), (1, 1), (3, 4), (3, 4),
            (2, 3), (3, 4), (3, 5), (3, 4), (1, 2),
            (3, 5), (5, 10), (3, 5), (1, 7), (4, 4),
            (4, 3), (4, 4), (3, 4), (3, 4), (2, 6),
            (4, 1), (4, 5), (4, 5), (4, 6), (3, 5),
            (3, 5), (3, 5), (1, 6), (3, 6), (2, 6),
            (2, 6), (1, 4), (1, 6), (1, 3), (2, 5),
            (2, 4), (4, 6), (2, 5), (3, 4), (2, 5),
            (1, 9), (1, 9), (1, 8), (3, 7), (2, 7),
            (2, 4), (2, 7), (4, 1), (4, 1), (4, 2),
            (2, 5), (4, 4), (2, 4), (2, 2)
        ];

    static List<(int, int)> Player2 =>
        [
            (5, 8), (5, 9), (6, 7), (4, 8), (3, 8),
            (4, 8), (4, 8), (4, 8), (4, 6), (5, 7),
            (5, 9), (3, 8), (3, 8), (5, 8), (5, 8),
            (5, 9), (4, 9), (4, 9), (4, 8), (4, 8),
            (4, 7), (4, 7), (5, 7), (4, 8), (3, 8),
            (4, 8), (3, 9), (3, 8), (4, 9), (3, 8),
            (2, 8), (2, 8), (2, 7), (3, 7), (3, 6),
            (4, 5), (2, 6), (3, 6), (4, 7), (4, 6),
            (3, 5), (4, 6), (4, 4), (4, 6), (4, 8),
            (3, 3), (2, 5), (4, 3), (3, 3), (6, 3),
            (5, 3), (4, 3), (4, 5)
        ];

    static IEnumerable<(int, int)> Items
    {
        get
        {
            var count = Math.Max(Player1.Count, Player2.Count);

            for (var i = 0; i < count; i++)
            {
                if (i < Player1.Count)
                {
                    yield return Player1[i];
                }

                if (i < Player2.Count)
                {
                    yield return Player2[i];
                }
            }
        }
    }

    public IEnumerator<(int, int)> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }
}
