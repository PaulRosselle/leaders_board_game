using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Actions;

public class CharacterAction : IGameAction
{
    public GameActionType ActionType => GameActionType.CharacterAction;

    public bool IsActiveAbility { get; }
    public CharacterActionDetail Source { get; }
    public List<CharacterActionDetail> Targets { get; }

    public CharacterAction(CharacterActionDetail source, List<CharacterActionDetail> targets, bool isActiveAbility)
    {
        Source = source;
        Targets = targets;
        IsActiveAbility = isActiveAbility;
    }
}