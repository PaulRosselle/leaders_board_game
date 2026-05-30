namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class IllusionistCharacterActionResolver : CharacterActionResolver
{
    public IllusionistCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    
    protected override List<Cell> GetActiveAbilityTargets()
    {
        List<Cell> targetCells = [];
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            Cell? targetCell = GetTargetInDirection(direction);
            if (targetCell is not null)
            {
                targetCells.Add(targetCell);
            }
        }
        return targetCells;
    }

    protected override List<CharacterAction> GenerateActiveAbilityActions()
    {
        List<CharacterAction> activeAbilityActions = [];
        // The active ability action generation moves by default the target
        // to a destination since it is the most whitespread behavior
        foreach (Cell targetCell in GetActiveAbilityTargets())
        {
            if (targetCell.Character is null)
            {
                throw new InvalidOperationException("Invalid active ability target : the targeted cell contains no character");
            }
            // The target takes the place of the illusionist and vice-versa
            CharacterActionTarget targetMovement = new CharacterActionTarget(targetCell.Character, targetCell.Pos, CharacterCell.Pos);
            CharacterActionTarget illusionistMovement = new CharacterActionTarget(Character, CharacterCell.Pos, targetCell.Pos);
            CharacterAction action = new CharacterAction(Character, [illusionistMovement, targetMovement], true);
            activeAbilityActions.Add(action);
        }

        return activeAbilityActions;
    }
    
    private Cell? GetTargetInDirection(Direction direction)
    {
        // The illusionist can target non-adjacent characters visibles in a straight line
        if (CharacterCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell))
        {
            Cell? targetCell = BoardQuery.FindFirstCellInDirectionMatchingCharacter(Game.Board, CharacterCell.Pos, direction, null, null);
            return adjacentCell != targetCell ? targetCell : null;
        }
        return null;
    }
}