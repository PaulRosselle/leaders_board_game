namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public class NemesisCharacterActionResolver : CharacterActionResolver
{
    public NemesisCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

     protected override int GetMovementMaxDistance()
    {
        // Special ability : the Nemesis movement max distance is 2
        return 2;
    }

    protected override List<Cell> GetMovementDestinations()
    {
        // Nemesis must move by two cells. If it is not possible, it only moves by 1 cell
        List<Cell> firstStepCells = [];
        HashSet<Cell> secondStepCells = [];

        // We get every cell at 1 and 2 distance from the Nemesis
        foreach (Direction firstStepDirection in Enum.GetValues<Direction>())
        {
            if (CharacterCell.AdjacentCells.TryGetValue(firstStepDirection, out Cell? firstStepCell) && firstStepCell.Character is null)
            {
                foreach (Direction secondStepDirection in Enum.GetValues<Direction>())
                {
                    if (firstStepCell.AdjacentCells.TryGetValue(secondStepDirection, out Cell? secondStepCell) && secondStepCell.Character is null)
                    {
                        // HashSet avoid duplicates automatically
                        secondStepCells.Add(secondStepCell);
                    }
                }
                firstStepCells.Add(firstStepCell);
            }
        }

        // If no cell is available at a 2 step distance, we return the 1 step cells
        if (secondStepCells.Count > 0)
        {
            // It is not allowed for the Nemesis to go back to its original position
            secondStepCells.Remove(CharacterCell);
            return [.. secondStepCells];
        }
        return firstStepCells;
    }
}