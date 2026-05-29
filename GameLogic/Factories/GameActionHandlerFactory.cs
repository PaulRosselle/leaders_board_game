namespace LeadersBoardGame.GameLogic.Factories;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Handlers;

public static class GameActionHandlerFactory
{
    public static IActionHandler Create(Game game, IGameAction action) => action.ActionType switch
    {
        GameActionType.Transition => new TransitionActionHandler(game, (TransitionAction)action),
        GameActionType.CharacterAction => new CharacterActionHandler(game, (CharacterAction)action),
        GameActionType.Recruitment => new RecruitmentActionHandler(game, (RecruitmentAction)action),
        GameActionType.Banishment => new BanishmentActionHandler(game, (BanishmentAction)action),
        _ => throw new InvalidOperationException($"No handler found for action type \"{action.ActionType}\""),
    };
}