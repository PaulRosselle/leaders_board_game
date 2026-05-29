namespace LeadersBoardGame.GameLogic.Factories;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Handlers;

public static class GameActionHandlerFactory
{
    public static IActionHandler Create(Game game, GameHistory history, IGameAction action) => action.ActionType switch
    {
        GameActionType.Transition => new TransitionActionHandler(game, history, (TransitionAction)action),
        GameActionType.CharacterAction => new CharacterActionHandler(game, history, (CharacterAction)action),
        GameActionType.Recruitment => new RecruitmentActionHandler(game, history, (RecruitmentAction)action),
        GameActionType.Banishment => new BanishmentActionHandler(game, history, (BanishmentAction)action),
        _ => throw new InvalidOperationException($"No handler found for action type \"{action.ActionType}\""),
    };
}