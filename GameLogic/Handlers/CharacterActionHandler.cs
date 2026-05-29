namespace LeadersBoardGame.GameLogic.Handlers;

using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Actions;
using System;

public class CharacterActionHandler : IActionHandler
{
    public Game Game { get; }

    protected CharacterAction Action { get; }

    public CharacterActionHandler(Game game, CharacterAction action)
    {
        Game = game;
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