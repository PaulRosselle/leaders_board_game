namespace LeadersBoardGame.GameLogic;

using System;
using System.Collections.Generic;
using System.Linq;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Factories;
using LeadersBoardGame.GameLogic.HistoryEntries;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;
using LeadersBoardGame.GameLogic.Queries;

public class GameHandler
{
    public Game CurrentGame { get; }

    public GameHistory CurrentHistory { get; }

    public GameHandler(GameHistory gameHistory)
    {
        CurrentGame = GameFactory.Create(gameHistory);
        CurrentHistory = gameHistory;
    }

    /// <summary>
    /// Returns the game mode for the current game
    /// </summary>
    public GameMode GetGameMode()
    {
        // TODO - add summary
        return CurrentHistory.Config.GameMode;
    }

    /// <summary>
    /// Returns the players for the current game
    /// </summary>
    public Player[] GetPlayers()
    {
        // TODO - add summary
        return CurrentHistory.Config.Players;
    }

    /// <summary>
    /// Returns the player with the matching team color
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no player with the given teamColor could be found
    /// </exception>
    private Player GetPlayerFromTeamColor(TeamColor teamColor)
    {
        foreach (Player player in GetPlayers())
        {
            if (player.Team == teamColor)
            {
                return player;
            }
        }
        throw new InvalidOperationException($"No player found for team {teamColor}");
    }

    /// <summary>
    /// Returns the player expected to perform the next action.
    /// During the actions phase, the current player is inferred from the playable characters team.
    /// Otherwise, falls back to the current history entry's team.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when playable characters belong to different teams, or when no current history entry is found.
    /// </exception>
    public Player GetCurrentPlayer()
    {
        // During the actions phase, the current player is defined by which characters are playable
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (currentPhase is ActionsPhase)
        {
            // First we try to get the playable characters team
            TeamColor? playableCharacterTeam = null;
            foreach (Cell characterCell in GetPlayableCharacters())
            {
                if (playableCharacterTeam == null)
                {
                    playableCharacterTeam = characterCell.Character!.Team;
                }
                else if (playableCharacterTeam != characterCell.Character!.Team)
                {
                    throw new InvalidOperationException("There should not be characters of different teams playable at the same time");
                }
            }
            // If we found a valid one, we can return the matching player. Otherwise we use the default behavior
            if (playableCharacterTeam is TeamColor playableTeam)
            {
                return GetPlayerFromTeamColor(playableTeam);
            }
        }

        // The default behavior is to return the current history entry team player
        TeamColor? currentEntryTeam = GameHistoryQuery.GetCurrentEntryTeam(CurrentHistory);
        if (currentEntryTeam is TeamColor currentPhaseTeam)
        {
            return GetPlayerFromTeamColor(currentPhaseTeam);
        }
        throw new InvalidOperationException("Cannot get the current player without history entries");
    }

    /// <summary>
    /// returns a GamePhase instance based on the current phase and current entry team
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no valid phase in progress can be found, or when the phase type has no GamePhaseType equivalent
    /// </exception>
    public GamePhase GetCurrentPhase()
    {
        // First we try to get the current phase and entry team
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        TeamColor? currentEntryTeam = GameHistoryQuery.GetCurrentEntryTeam(CurrentHistory);
        if (currentEntryTeam is TeamColor currentPhaseTeam && currentPhase is ISegment currentSegment)
        {
            // Transition target should contain an equivalent for every GamePhaseType
            GamePhaseType gamePhaseType = currentSegment.Transition switch
            {
                TransitionTarget.BanishmentPhase => GamePhaseType.Banishment,
                TransitionTarget.TurnStartPhase => GamePhaseType.TurnStart,
                TransitionTarget.ActionsPhase => GamePhaseType.Actions,
                TransitionTarget.RecruitmentPhase => GamePhaseType.Recruitment,
                TransitionTarget.TurnEndPhase => GamePhaseType.TurnEnd,
                _ => throw new InvalidOperationException($"{currentSegment.Transition} isn't a valid phase type"),
            };
            // Finally we return a "GamePhase" using the found type and player
            return new GamePhase(gamePhaseType, GetPlayerFromTeamColor(currentPhaseTeam));
        }
        throw new InvalidOperationException("No phase in progress found");
    }

    /// <summary>
    /// Returns the player who has won the game. Returns null if there are no such player
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no current history entry is found.
    /// </exception>
    public Player? GetWinner()
    {
        // First, we get the team color for both players
        TeamColor? currentEntryTeam = GameHistoryQuery.GetCurrentEntryTeam(CurrentHistory);
        if (currentEntryTeam is not TeamColor currentPhaseTeam)
        {
            throw new InvalidOperationException("Cannot get the current player with a valid history entry");
        }
        TeamColor opponentTeam = currentPhaseTeam.GetOpposite();

        TeamColor? winnerTeam = null;


        // TODO - move out the warningAction creation out of this function and test only the two warnings ?
        // At the end of a turn, we must check if a barrage is detected for the previous player
        IPhase? phase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (phase is TurnEndPhase turnEndPhase)
        {
            // If a barrage is detected, a warning is given to the player
            // TODO - Add WarningAction class and implement Barrage actions
            // When a player holds a barrage during two turn, they immediately lose the game
            // TODO - make currentPhaseTeam player win if a warning was already given
        }

        // We check if the current leader is captured/surrounded first to stay consistent with 
        // the "don't capture your own leader" rule
        if (GameQuery.IsLeaderCaptured(CurrentGame, CurrentHistory, currentPhaseTeam) ||
            GameQuery.IsLeaderSurrounded(CurrentGame, currentPhaseTeam))
        {
            winnerTeam = currentPhaseTeam;
        }
        else if (GameQuery.IsLeaderCaptured(CurrentGame, CurrentHistory, currentPhaseTeam) ||
                GameQuery.IsLeaderSurrounded(CurrentGame, currentPhaseTeam))
        {
            winnerTeam = opponentTeam;
        }

        // If a winner team was found, we return the associated player
        if (winnerTeam is TeamColor winnerPlayerTeam)
        {
            return GetPlayerFromTeamColor(winnerPlayerTeam);
        }

        // Without a winner, we return null
        return null;
    }

    public List<Cell> GetPlayableCharacters()
    {
        // TODO - add summary
        return PlayabilityQuery.GetPlayableCharacters(CurrentGame, CurrentHistory);
    }

    public List<CharacterAction> GetMovementActions(Character character)
    {
        // TODO - add summary
        throw new NotImplementedException();
    }

    public List<CharacterAction> GetActiveAbilityActions(Character character)
    {
        // TODO - add summary
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the number of cards that can be recruited during the current recruitment phase
    /// </summary>
    private int GetRecruitmentLimit()
    {
        // The 2nd player is allowed to recruit twice during their first turn
        IHistoryEntry currentEntry = GameHistoryQuery.GetLastEntry(CurrentHistory);
        foreach (IHistoryEntry historyEntry in CurrentHistory.Entries)
        {
            if (historyEntry is Turn turn && turn.Team != CurrentHistory.Config.FirstPlayer.Team)
            {
                return turn == currentEntry ? 2 : 1;
            }
        }

        // A full team is composed of 5 character cards (a leader card + 4 recruited cards)
        const int teamMaxSize = 5;
        // The "RecruitedCharacters" list tracks every character added through a "RecruitmentAction".
        // This list takes into account the leaders since they're added from the config using RecruitmentAction
        HashSet<CharacterCard> cardsInTeam = [];
        foreach (Character recruitedCharacter in CurrentGame.RecruitedCharacters)
        {   
            if (recruitedCharacter.Team == currentEntry.Team)
            {
                cardsInTeam.Add(recruitedCharacter.CharacterType.GetCharacterCard());
            }
        }
        // If a character can still be recruited, we return 1
        if (cardsInTeam.Count < teamMaxSize)
        {
            return 1;
        }

        // When the team is already full, we return 0
        return 0;
    }

    /// <summary>
    /// Returns every character type matching "card"
    /// </summary>
    private List<CharacterType> GetCharacterTypesMatchingCard(CharacterCard card)
    {
        List<CharacterType> characterTypes = [];
        foreach (CharacterType characterType in CharacterTypeExtension.AllCharacterTypes)
        {
            if (characterType.GetCharacterCard() == card)
            {
                characterTypes.Add(characterType);
            }
        }
        return characterTypes;
    }

    /// <summary>
    /// Returns the list of cards recruitable during the current recruitment phase
    /// </summary>
    public List<CharacterCard> GetRecruitableCards()
    {
        // First we get the team associated with the current entry since it will define the recruitment cells
        TeamColor? currentEntryTeam = GameHistoryQuery.GetCurrentEntryTeam(CurrentHistory);
        if (currentEntryTeam is not TeamColor currentRecruitmentTeam)
        {
            throw new InvalidOperationException("Cannot recruit without a valid history entry");
        }
        // No recruitment can be made without at least one available recruitment cell
        List<Cell> recruitmentCells = BoardQuery.GetRecruitmentCells(CurrentGame.Board, currentRecruitmentTeam);
        if (recruitmentCells.Count == 0)
        {
            return [];
        }

        // We only return a list if a recruitment phase is in progress
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (currentPhase is RecruitmentPhase recruitmentPhase)
        {
            // We get every card already recruited this phase
            HashSet<CharacterCard> recruitedCharacterCard = [];
            foreach (RecruitmentAction recruitmentAction in recruitmentPhase.Actions.Cast<RecruitmentAction>())
            {
                recruitedCharacterCard.Add(recruitmentAction.Character.CharacterType.GetCharacterCard());
            }
            // If the recruited card cound is still under the limit, we can return every card with 
            // characters able to be placed on the board
            if (recruitedCharacterCard.Count < GetRecruitmentLimit())
            {
                List<CharacterCard> recruitableCards = [];
                foreach (CharacterCard recruitableCard in CurrentGame.RecruitableCards)
                {
                    if (GetCharacterTypesMatchingCard(recruitableCard).Count <= recruitmentCells.Count)
                    {
                        recruitableCards.Add(recruitableCard);
                    }                    
                }
                return recruitableCards;
            }
        }
        // By default we return an empty list
        return [];
    }

    /// <summary>
    /// Returns the first character type enum linked with "card" able to be recruited.
    /// A character is considered recruitable if it hasn't already been recruited
    /// </summary>
    public CharacterType? GetRecruitableCharacterType(CharacterCard card)
    {
        // First we get the character types already recruited during the current phase
        List<CharacterType> recruitedCharacterTypes = [];
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (currentPhase is RecruitmentPhase recruitmentPhase)
        {
            foreach (RecruitmentAction recruitmentAction in recruitmentPhase.Actions.Cast<RecruitmentAction>())
            {
                recruitedCharacterTypes.Add(recruitmentAction.Character.CharacterType);
            }
        }

        // Then we try to get the first character type matching the card outside of "recruitedCharacterTypes"
        CharacterType? recruitableCharacterType = null;
        foreach (CharacterType characterType in GetCharacterTypesMatchingCard(card))
        {
            if (!recruitedCharacterTypes.Contains(characterType))
            {
                recruitableCharacterType = characterType;
            }
        }
        return recruitableCharacterType;
    }

    /// <summary>
    /// Returns the recruitment actions for a character linked to the card.
    /// If multiple characters are linked to the card, return actions for the first non
    /// already recruited character matching the card in the characterType enum
    /// </summary>
    public List<RecruitmentAction> GetRecruitmentActions(CharacterCard card)
    {
        // First, we get the current history entry team color since this will define
        // the recruited character color and the recruitment cells
        TeamColor? currentEntryTeam = GameHistoryQuery.GetCurrentEntryTeam(CurrentHistory);
        if (currentEntryTeam is not TeamColor currentRecruitmentTeam)
        {
            throw new InvalidOperationException("Cannot recruit a character without a valid history entry");
        }

        // Then we try to get the recruitment cells and recruitable character type
        List<Cell> recruitmentCells = BoardQuery.GetRecruitmentCells(CurrentGame.Board, currentRecruitmentTeam);
        CharacterType? recruitableCharacterType = GetRecruitableCharacterType(card);

        // If we have a valid character and recruitment cells, we can generate the recruitment actions
        List<RecruitmentAction> recruitmentActions = [];
        if (recruitmentCells.Count > 0 && recruitableCharacterType is CharacterType recruitedCharacterType)
        {
            // We generate here a new Character instance to be used in the history and the game projection
            Character recruitedCharacter = CharacterFactory.Create(recruitedCharacterType, currentRecruitmentTeam);
            // Finally we generate an action for each 
            foreach (Cell recruitmentCell in recruitmentCells)
            {
                recruitmentActions.Add(new RecruitmentAction(recruitedCharacter, recruitmentCell.Pos));
            }
        }

        return recruitmentActions;
    }

    /// <summary>
    /// Returns the list of cards banishable during the current banishment phase
    /// </summary>
    public List<CharacterCard> GetBanishableCards()
    {
        // Any recruitable card can be banished during a banishment phase
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (currentPhase is BanishmentPhase)
        {
            return CurrentGame.RecruitableCards;
        }
        return [];
    }

    /// <summary>
    /// Returns the banishment action for a given card
    /// </summary>
    public BanishmentAction GetBanishmentAction(CharacterCard card)
    {
        return new BanishmentAction(card, GetCurrentPhase().PhasePlayer.Team);
    }

    public void DoAction(IGameAction action)
    {
        // TODO - add summary
        throw new NotImplementedException();
    }

    public bool CanUndoAction()
    {
        // TODO - add summary
        throw new NotImplementedException();
    }

    public void UndoLastAction()
    {
        // TODO - add summary
        throw new NotImplementedException();
    }

    public bool CanSkipCurrentPhase()
    {
        // TODO - add summary
        throw new NotImplementedException();
    }

    public void AdvancePhase()
    {
        // TODO - summary
        throw new NotImplementedException();
    }
}