namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Queries;

public class RoyalGuardCharacterActionResolver : CharacterActionResolver
{
    public RoyalGuardCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        // The royal guard's active ability only targets itself
        return [CharacterCell];
    }

    protected override List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        // The royal guard's active ability allows him to move up to two tiles around its leader
        Cell? leaderCell = BoardQuery.FindLeaderCell(Game.Board, Character.Team);
        if (leaderCell is not null)
        {
            return GetAdjacentEmptyCells(leaderCell.Pos, 2);
        }
        return [];
    }
}