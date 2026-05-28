using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Actions;
using System;
using LeadersBoardGame.GameLogic.Queries;
using System.Collections.Generic;

namespace LeadersBoardGame.GameLogic.Handlers;

public abstract class CharacterActionHandler : IActionHandler
{
    protected Game _game;
    public Game Game => _game;
    protected CharacterAction Action { get; }

    public CharacterActionHandler(Game game, CharacterAction action)
    {
        _game = game;
        Action = action;
    }

    /// <summary>
    /// Applies the Action effects to the game
    /// </summary>
    public virtual void DoAction()
    {
        throw new NotImplementedException();
    }

    // <summary>
    /// Reverts the Action effects on the game
    /// </summary>
    public virtual void UndoAction()
    {
        throw new NotImplementedException();
    }
}