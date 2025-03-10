using DTO = Atoms.Core.DTOs.GameDTO;

namespace Atoms.UnitTests.Core.DTOs.GameDTO;

public class FromEntityTests
{
    [Test, MethodDataSource(nameof(TestCases))]
    public async Task ShouldMapAllFieldsCorrectly(Game game, DTO expected)
    {
        var dto = DTO.FromEntity(game);

        await Assert.That(dto).IsEquivalentTo(expected);
    }

    public static IEnumerable<Func<(Game, DTO)>> TestCases()
    {
        yield return () => (ObjectMother.Game(), ObjectMother.GameDTO());

        yield return () => 
        (
            ObjectMother.Game(
                cells: [new(1, 1, 1, 1), new(1, 2, 2, 2)],
                move: 3,
                round: 2), 
            ObjectMother.GameDTO(
                move: 3, 
                round: 2,
                board: ObjectMother.BoardDTO("1-1-1-1,1-2-2-2"))
        );
    }
}
