namespace LeadersBoardGame.GameLogic.Handlers;

using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Actions;
using System;

public class CharacterActionHandler : IActionHandler
{
    public Game Game { get; }

    public GameHistory History { get; }

    protected CharacterAction Action { get; }

    public CharacterActionHandler(Game game, GameHistory history, CharacterAction action)
    {
        Game = game;
        History = history;
        Action = action;
    }

    /// <summary>
    /// Applies the Action effects to the game
    /// </summary>
    public void DoAction()
    {
        throw new NotImplementedException();
    }

    // <summary>
    /// Reverts the Action effects on the game
    /// </summary>
    public void UndoAction()
    {
        throw new NotImplementedException();
    }
}