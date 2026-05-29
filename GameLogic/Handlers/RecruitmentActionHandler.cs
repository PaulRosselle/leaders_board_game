namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class RecruitmentActionHandler : IActionHandler
{
    public Game Game { get; }

    public RecruitmentAction Action { get; }

    public RecruitmentActionHandler(Game game, RecruitmentAction action)
    {
        Game = game;
        Action = action;
    }

    public void DoAction()
    {
        BoardQuery.GetCell(Game.Board, Action.DestPos).Character = Action.Character;
        // We always remove the card matching the recruited character from the recruitable cards pool.
        // The only cases where the removal can fail (without generating any exception) are 
        // cards like the Hermit & Cub matching multiple characters
        Game.RecruitableCards.Remove(Action.Character.CharacterType.GetCharacterCard());
        Game.RecruitedCharacters.Add(Action.Character);
    }

    public void UndoAction()
    {
        Game.RecruitedCharacters.Remove(Action.Character);
        // We only add back a card once into the recruitable cards pool since it shouldn't host any duplicate
        CharacterCard card = Action.Character.CharacterType.GetCharacterCard();
        if (!Game.RecruitableCards.Contains(card))
        {
            Game.RecruitableCards.Add(card);
        }
        BoardQuery.GetCell(Game.Board, Action.DestPos).Character = null;
    }
}