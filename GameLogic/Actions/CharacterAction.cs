using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Actions;

public class CharacterAction : IGameAction
{
    public GameActionType ActionType => GameActionType.CharacterAction;

    public bool IsActiveAbility { get; }
    public Character SourceCharacter { get; }
    public List<CharacterActionTarget> Targets { get; }

    public CharacterAction(Character sourceCharacter, List<CharacterActionTarget> targets, bool isActiveAbility)
    {
        SourceCharacter = sourceCharacter;
        Targets = targets;
        IsActiveAbility = isActiveAbility;
    }
}