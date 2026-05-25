using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

namespace LeadersBoardGame.GameLogic.Handlers;

public class RecruitmentActionHandler : IActionHandler
{
    private Game _game;
    public Game Game => _game;
    public RecruitmentAction Action { get; }

    public RecruitmentActionHandler(Game game, RecruitmentAction action)
    {
        _game = game;
        Action = action;
    }

    public void DoAction()
    {
        // We add the recruited character to its destination on the board (which must be empty)
        BoardQuery.GetCell(Game.Board, Action.DestPos).Character = Action.Character;
        // Then we remove immediately the character card from the recruitable cards
        CharacterCard card = Action.Character.CharacterType.GetCharacterCard();
        if (_game.RecruitableCards.Contains(card))
        {
            _game.RecruitableCards.Remove(card);
        }

        _game.History.Add(Action);
    }

    public void UndoAction()
    {
        // We empty the character destination on the board since cancelling a
        // recruitment goes with removing the correspond character on the board
        BoardQuery.GetCell(Game.Board, Action.DestPos).Character = null;
        // We add back the character from the recruitable cards if needed
        CharacterCard card = Action.Character.CharacterType.GetCharacterCard();
        if (!_game.RecruitableCards.Contains(card))
        {
            // Since cards are picked from the start of the list in Discovery
            // mode, we add back the recruited card at index zero
            _game.RecruitableCards.Insert(0, card);
        }

        _game.History.Remove(Action);
    }
}