using Atoms.Core.DTOs;
using Atoms.Core.Entities;

namespace Atoms.Core.Interfaces;

public interface IGameFactory
{
    Game Create(GameMenuOptions menuDto);
}
