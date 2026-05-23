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
    /// Returns as a single list every action detail of the handled action
    /// </summary>
    /// <returns></returns>
    protected List<CharacterActionDetail> GetAllDetails()
    {
        List<CharacterActionDetail> allDetails = [Action.Source, .. Action.Targets];
        return allDetails;
    }

    /// <summary>
    /// Applies the Action effects to the game
    /// </summary>
    public virtual void DoAction()
    {
        // The default bevahior is to move every character with an action detail to its destination.
        List<CharacterActionDetail> actionDetails = GetAllDetails();
        // First, we remove the pieces with a valid destination from their original position
        foreach (CharacterActionDetail actionDetail in actionDetails)
        {
            if (actionDetail.DestPos is not null)
            {
                BoardQuery.GetCell(Game.Board, actionDetail.OriginPos).Character = null;
            }
        }
        // Then we add them to their destination
        foreach (CharacterActionDetail actionDetail in actionDetails)
        {
            if (actionDetail.DestPos is not null)
            {
                BoardQuery.GetCell(Game.Board, actionDetail.DestPos).Character = actionDetail.Character;
            }
        }
        // Finally, we add the action to the game's history
        Game.History.Add(Action);
    }

    // <summary>
    /// Reverts the Action effects on the game
    /// </summary>
    public virtual void UndoAction()
    {
        // The default behavior is to move back every character with an action detail to its original position.
        List<CharacterActionDetail> actionDetails = GetAllDetails();
        // First, we remove the pieces from their destination
        foreach (CharacterActionDetail actionDetail in actionDetails)
        {
            if (actionDetail.DestPos is not null)
            {
                BoardQuery.GetCell(Game.Board, actionDetail.DestPos).Character = null;
            }
        }
        // Then we add them back to their original position
        foreach (CharacterActionDetail actionDetail in actionDetails)
        {
            BoardQuery.GetCell(Game.Board, actionDetail.OriginPos).Character = actionDetail.Character;
        }
        // Finally, we remove the action from the game's history
        Game.History.Remove(Action);
    }
}