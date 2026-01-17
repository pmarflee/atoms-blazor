using Atoms.Core.DTOs.Notifications;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.Services;

public class GameMoveService : IGameMoveService
{
    public async Task PlayAllMoves(
        Game game, Cell? cell = null, Notify? notify = null)
    {
        if (game.HasWinner) return;

        var requestPlayer = game.ActivePlayer;

        if (requestPlayer.IsHuman && cell is null)
        {
            throw new Exception("Player must select a cell");
        }

        do
        {
            await PlayMove(game, cell, requestPlayer, notify: notify);
        } while (!game.HasWinner && !game.ActivePlayer.IsHuman);
    }

    public async Task PlayMove(
        Game game, Cell? cell, 
        Game.Player? requestPlayer = null,
        bool debug = false, Notify? notify = null)
    {
        var player = game.ActivePlayer;

        if (!player.IsHuman)
        {
            cell = player.ChooseCell(game);
        }

        if (cell is not null)
        {
            await PlaceAtom(game, cell, requestPlayer, notify);

            var overloaded = new Stack<Cell>();

            if (cell.IsOverloaded)
            {
                overloaded.Push(cell);

                await DoChainReaction(game, overloaded, requestPlayer, notify);
            }
        }

        if (!game.HasWinner)
        {
            game.PostMoveUpdate();
        }

        if (!debug && notify is not null)
        {
            await NotifyPlayerMoved(game, player, requestPlayer, notify);
        }
    }

    static async Task DoChainReaction(
        Game game, Stack<Cell> overloaded,
        Game.Player? requestPlayer, Notify? notify)
    {
        do
        {
            var cell = overloaded.Pop();

            if (!cell.IsOverloaded) continue;

            await DoExplosion(game, cell, requestPlayer, notify);

            foreach (var neighbour in game.Board.GetNeighbours(cell))
            {
                await PlaceAtom(game, neighbour, requestPlayer, notify);

                if (neighbour.Atoms == neighbour.MaxAtoms + 1)
                {
                    overloaded.Push(neighbour);
                }
            }

            game.CheckForWinner();

            if (game.HasWinner) break;
        } while (overloaded.Count > 0);
    }

    static async Task PlaceAtom(
        Game game, Cell cell, 
        Game.Player? requestPlayer, Notify? notify)
    {
        game.PlaceAtom(cell);

        async Task Notify()
        {
            if (notify is not null)
            {
                await NotifyAtomPlaced(game, cell, requestPlayer, notify);
            }
        }

        async Task SetHighlightAndNotify(bool highlight)
        {
            cell.Highlighted = highlight;

            await Notify();
        }

        await SetHighlightAndNotify(true);
        await SetHighlightAndNotify(false);
    }

    static async Task DoExplosion(
        Game game, Cell cell,
        Game.Player? requestPlayer, Notify? notify)
    {
        cell.Explosion = ExplosionState.Before;
        cell.Explode();

        if (notify is not null)
        {
            await NotifyAtomExploded(game, cell, requestPlayer, notify);
        }

        cell.Explosion = ExplosionState.After;

        if (notify is not null)
        {
            await NotifyAtomExploded(game, cell, requestPlayer, notify);
        }

        cell.Explosion = ExplosionState.None;

        if (notify is not null)
        {
            await NotifyAtomExploded(game, cell, requestPlayer, notify);
        }
    }

    static async Task NotifyAtomPlaced(
        Game game, Cell cell,
        Game.Player? requestPlayer, Notify notify)
    {
        await notify.Invoke(
            new AtomPlaced(game.Id,
                           game.ActivePlayer.Id,
                           requestPlayer?.Id,
                           cell.Row,
                           cell.Column));
    }

    static async Task NotifyAtomExploded(
        Game game, Cell cell, 
        Game.Player? requestPlayer, Notify notify)
    {
        await notify.Invoke(
            new AtomExploded(game.Id,
                             game.ActivePlayer.Id,
                             requestPlayer?.Id,
                             cell.Row,
                             cell.Column,
                             cell.Explosion));
    }

    static async Task NotifyPlayerMoved(
        Game game, Game.Player player, 
        Game.Player? requestPlayer, Notify notify)
    {
        await notify.Invoke(new PlayerMoved(game.Id, player.Id, requestPlayer?.Id));
    }
}
