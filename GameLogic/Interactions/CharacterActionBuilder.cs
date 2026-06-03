namespace LeadersBoardGame.GameLogic.Interactions;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;

public class CharacterActionBuilder
{
    private readonly List<InteractionResult> _interactionResults;

    public IReadOnlyList<InteractionResult> InteractionResults => _interactionResults;
    public Character SourceCharacter { get; }

    public CharacterActionBuilder(Character sourceCharacter)
    {
        _interactionResults = [];
        SourceCharacter = sourceCharacter;
    }

    public void AddResult(InteractionResult result)
    {
        _interactionResults.Add(result);
    }
}
