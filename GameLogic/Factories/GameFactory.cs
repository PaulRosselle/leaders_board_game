namespace LeadersBoardGame.GameLogic.Factories;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.HistoryEntries;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;

public static class GameFactory
{
    public static Game Create(GameHistory gameHistory)
    {
        // First, we create the game instance with the default board and initial recruitable cards
        Board board = BoardFactory.Create();
        List<CharacterCard> recruitableCards = [.. gameHistory.Config.InitialRecruitableCards];
        List<Character> recruitedCharacters = [];
        List<CharacterCard> banishedCards = [];
        Game game = new Game(board, recruitableCards, recruitedCharacters, banishedCards);

        // The config contains every actions made before the game started (for initialization)
        DoActions(game, gameHistory, [.. gameHistory.Config.InitialPlacements]);

        // Once the game has been initialized fully, we can play every action in the history
        foreach (IHistoryEntry historyEntry in gameHistory.Entries)
        {
            // We ignore transition actions since they have no impact on the Game projection
            if (historyEntry is Turn turn)
            {
                // Actions in a turn are always played in this order : Start, ActionsPhase, RecruitmentPhase, End
                DoActions(game, gameHistory, turn.TurnStart.Actions);
                DoActions(game, gameHistory, turn.Actions.Actions);
                DoActions(game, gameHistory, turn.Recruitment.Actions);
                DoActions(game, gameHistory, turn.TurnEnd.Actions);
            }
            else if (historyEntry is BanishmentPhase banishmentPhase)
            {
                DoActions(game, gameHistory, banishmentPhase.Actions);
            }
        }

        return game;
    }

    private static void DoActions(Game game, GameHistory gameHistory, List<IGameAction> actions)
    {
        foreach (IGameAction gameAction in actions)
        {
            GameActionHandlerFactory.Create(game, gameHistory, gameAction).DoAction();
        }
    }
}