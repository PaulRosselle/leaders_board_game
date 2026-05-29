namespace LeadersBoardGame.GameLogic.Handlers;

using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Queries;

/// <summary>
/// Handles character actions on the game board.
/// 
/// Four target cases are handled :
/// 1. movement (OriginPos → DestPos)
/// 2. addition (null → DestPos) - the character instance already exists and is simply placed
///    on the board, unlike a RecruitmentAction which also updates the recruitable pool
/// 3. removal (OriginPos → null)
/// 4. targeting without movement (null → null) - no treatment
/// 
/// Removals are always processed before additions to handle cases where two targets swap positions,
/// preventing a character from being removed from its destination after being placed there
/// </summary>
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
        // First we try to remove the character from its original position
        foreach (CharacterActionTarget actionTarget in Action.Targets)
        {
            if (actionTarget.OriginPos is not null)
            {
                BoardQuery.GetCell(Game.Board, actionTarget.OriginPos).Character = null;
            }
        }

        // Then we try to add it to its destination
        foreach (CharacterActionTarget actionTarget in Action.Targets)
        {
            if (actionTarget.DestPos is not null)
            {
                BoardQuery.GetCell(Game.Board, actionTarget.DestPos).Character = actionTarget.Character;
            }
        }
    }

    // <summary>
    /// Reverts the Action effects on the game
    /// </summary>
    public void UndoAction()
    {
        // First we try to remove the character from its destination
        foreach (CharacterActionTarget actionTarget in Action.Targets)
        {
            if (actionTarget.DestPos is not null)
            {
                BoardQuery.GetCell(Game.Board, actionTarget.DestPos).Character = null;
            }
        }

        // Then we try to restore it to its original position
        foreach (CharacterActionTarget actionTarget in Action.Targets)
        {
            if (actionTarget.OriginPos is not null)
            {
                BoardQuery.GetCell(Game.Board, actionTarget.OriginPos).Character = actionTarget.Character;
            }
        }
    }
}