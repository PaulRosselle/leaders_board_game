namespace LeadersBoardGame.GameLogic;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Factories;
using LeadersBoardGame.GameLogic.Handlers;
using LeadersBoardGame.GameLogic.HistoryEntries;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;
using LeadersBoardGame.GameLogic.Interactions;
using LeadersBoardGame.GameLogic.Queries;

/// <summary>
/// Single entry point for all external callers. Owns both the <see cref="GameHistory"/> and
/// the <see cref="Game"/> projection. Executes the game loop sequentially, firing events on
/// the injected <see cref="IGameFlowListener"/> and awaiting each response before proceeding.
/// Manages the lifecycle of <see cref="CharacterActionBuilder"/> instances and coordinates
/// appending to <see cref="GameHistory"/>.
/// </summary>
public class GameHandler
{
    private readonly IGameFlowListener _gameFlowListener;
    private CharacterActionBuilder? _characterActionBuilder;

    public Game CurrentGame { get; }

    public GameHistory CurrentHistory { get; }

    public GameHandler(GameHistory gameHistory, IGameFlowListener gameFlowListener)
    {
        CurrentGame = GameFactory.Create(gameHistory);
        CurrentHistory = gameHistory;
        _gameFlowListener = gameFlowListener;
        _characterActionBuilder = null;
    }

    /// <summary>
    /// TODO - explain
    /// </summary>
    private class GameEndedException : Exception
    {
        public Player Winner { get; }

        public GameEndedException(Player winner)
        {
            Winner = winner;
        }
    }
    
    /// <summary>
    /// Returns the game mode for the current game
    /// </summary>
    public GameMode GetGameMode()
    {
        return CurrentHistory.Config.GameMode;
    }

    /// <summary>
    /// Returns the players for the current game
    /// </summary>
    public Player[] GetPlayers()
    {
        return CurrentHistory.Config.Players;
    }

    // TODO - remove ?
    private TeamColor GetCurrentPhaseTeam()
    {
        TeamColor? currentEntryTeam = GameHistoryQuery.GetLastEntryTeam(CurrentHistory);
        if (currentEntryTeam is TeamColor currentPhaseTeam)
        {
            return currentPhaseTeam;
        }
        throw new InvalidOperationException("Impossible to get the last history entry team");
    }

    /// <summary>
    /// Returns a GamePhase instance based on the current phase and current entry team.
    /// Returns null if no IPhase is in progress.
    /// </summary>
    private GamePhase? GetCurrentPhase()
    {
        // First we try to get the current phase and entry team
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        TeamColor? currentEntryTeam = GameHistoryQuery.GetLastEntryTeam(CurrentHistory);
        if (currentEntryTeam is TeamColor currentPhaseTeam && currentPhase is Segment currentSegment)
        {
            return new GamePhase(
                GamePhaseTypeExtension.GetFromTransitionTarget(currentSegment.Transition), 
                GameHistoryQuery.GetPlayerFromTeam(CurrentHistory, currentPhaseTeam));
        }
        return null;
    }

    // TODO - async Task methods for
    // OnAutomaticActionExecuted
    // OnInputRequired

    public async Task RunAsync()
    {
        try
        {
            await _gameFlowListener.OnGameStarted(CurrentGame);
            // We loop between game phases until the game ends (with an GameEndedException)
            while (true)
            {
                GamePhase? phase = GetCurrentPhase();
                if (phase is GamePhase currentPhase)
                {
                    await RunCurrentPhaseAsync(currentPhase);
                } else
                {
                    // Without a phase in progress, we start the next one
                    await StartNextPhaseAsync();
                }
            }
        }
        catch (GameEndedException e)
        {
            await _gameFlowListener.OnGameEnded(e.Winner);
        }
    }

    public async Task RunCurrentPhaseAsync(GamePhase currentPhase)
    {
        switch (currentPhase.PhaseType)
        {
            case GamePhaseType.TurnStart: await RunTurnStartPhaseAsync(currentPhase); break;
            case GamePhaseType.Actions: await RunActionsPhaseAsync(currentPhase); break;
            case GamePhaseType.Recruitment: await RunRecruitmentPhaseAsync(currentPhase); break;
            case GamePhaseType.TurnEnd: await RunTurnEndPhaseAsync(currentPhase); break;
            case GamePhaseType.Banishment: await RunBanishmentPhaseAsync(currentPhase); break;
            default: throw new InvalidOperationException($"Invalid phase {currentPhase.PhaseType}");
        }
        EndCurrentPhase();
    }

    /// <summary>
    /// TODO
    /// </summary>
    public async Task RunTurnStartPhaseAsync(GamePhase currentPhase)
    {
        // Game ended state is always checked at the start of a turn
        CheckGameEnded(currentPhase);
    }

    public async Task RunActionsPhaseAsync(GamePhase currentPhase)
    {
        // Ask input until a InteractionResult.PhaseEnd is received
        bool endActionsPhase = false;
        do
        {
            // TODO - take into account UndoLastAction properly (there must be an undoable action)
            List<InteractionResultType> legalResults = [
                InteractionResultType.PositionChosen, 
                InteractionResultType.UndoLastAction,
                InteractionResultType.EndPhase];
            InteractionResult playableCharacterResult = await RunSelectPlayableCharacterAsync(legalResults);
            switch (playableCharacterResult.ResultType)
            {
                case InteractionResultType.PositionChosen: await RunPlayCharacterAsync(currentPhase, playableCharacterResult.ChosenPosition); break;
                case InteractionResultType.UndoLastAction: UndoLastAction(); break;
                case InteractionResultType.EndPhase: endActionsPhase = true; break;
                default: throw new InvalidOperationException($"Invalid interaction result : illegal type \"{playableCharacterResult.ResultType}\" for banishment");
            }
            // TODO - comment
            _characterActionBuilder = null;
        } while (!endActionsPhase);
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// TODO</exception>
    public async Task RunRecruitmentPhaseAsync(GamePhase currentPhase)
    {
        // While recruitment are needed, send request
        while (RecruitmentQuery.CanRecruit(CurrentHistory, CurrentGame, currentPhase.PhasePlayer.Team))
        {
            // First, we build the card selection request
            List<CharacterCard> recruitableCards = RecruitmentQuery.GetRecruitableCards(CurrentHistory, CurrentGame);
            List<InteractionResultType> cardSelectionLegalResults = [InteractionResultType.CardChosen];
            InteractionRequest cardSelectionRequest = new InteractionRequest(InteractionType.CharacterCardExpected, recruitableCards, null, cardSelectionLegalResults);
            // Then we send it to the listener to get a valid input 
            InteractionResult cardSelectionResult = await _gameFlowListener.OnInputRequired(cardSelectionRequest);

            if (cardSelectionResult.ResultType != InteractionResultType.CardChosen)
            {
                throw new InvalidOperationException($"Invalid interaction result : illegal type \"{cardSelectionResult.ResultType}\" for recruitment card selection");
                
            }
            if (cardSelectionResult.ChosenCard is not CharacterCard recruitedCard)
            {
                throw new InvalidOperationException("Invalid interaction result : chosen card missing for recruitment");
            }
            List<CharacterType> characterTypes = CharacterTypeExtension.GetCharacterTypesMatchingCard(recruitedCard);
            await RunRecruitmentCharacterAsync(currentPhase, characterTypes, 0);
        }
    }

    public async Task RunTurnEndPhaseAsync(GamePhase currentPhase)
    {
        // automatic turn end actions
        // barrage rules & throw GameEndedException if a player receive a 2nd warning
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    public async Task RunBanishmentPhaseAsync(GamePhase currentPhase)
    {
        // First, we build the banishment request
        List<CharacterCard> banishableCards = BanishmentQuery.GetBanishableCards(CurrentGame);
        List<InteractionResultType> legalResults = [InteractionResultType.CardChosen];
        InteractionRequest banishmentRequest = new InteractionRequest(InteractionType.CharacterCardExpected, banishableCards, null, legalResults);
        // Then we send it to the listener to get a valid input 
        InteractionResult banishmentResult = await _gameFlowListener.OnInputRequired(banishmentRequest);
        if (banishmentResult.ResultType != InteractionResultType.CardChosen)
        {
            throw new InvalidOperationException($"Invalid interaction result : illegal type \"{banishmentResult.ResultType}\" for banishment");
        }
        // Then we apply the banishment
        if (banishmentResult.ChosenCard is CharacterCard banishedCard)
        {
            DoAction(currentPhase, new BanishmentAction(banishedCard, currentPhase.PhasePlayer.Team));
        } else
        {
            throw new InvalidOperationException("Invalid interaction result : chosen card missing for banishment");
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <exception cref="GameEndedException">
    /// </exception>
    private void CheckGameEnded(GamePhase currentPhase)
    {
        TeamColor? winnerTeam = GameQuery.GetWinnerTeam(CurrentHistory, CurrentGame, currentPhase.PhasePlayer.Team);
        if (winnerTeam is TeamColor winnerPlayerTeam)
        {
            throw new GameEndedException(GameHistoryQuery.GetPlayerFromTeam(CurrentHistory, winnerPlayerTeam));
        }
    }

    private async Task<InteractionResult> RunSelectPlayableCharacterAsync(List<InteractionResultType> legalResults)
    {
        List<Position> playableCharacterPositions = PlayabilityQuery.GetPlayableCharacters(CurrentGame, CurrentHistory);
        Dictionary<TargetCategory, IReadOnlyList<Position>> legalPositions = [];
        legalPositions.Add(TargetCategory.PlayableCharacter, playableCharacterPositions);
        InteractionRequest playableCharacterRequest = new InteractionRequest(InteractionType.PositionExpected, null, legalPositions, legalResults);
        return await _gameFlowListener.OnInputRequired(playableCharacterRequest);
    }

    public async Task RunPlayCharacterAsync(GamePhase currentPhase, Position? characterPosition)
    {
        if (characterPosition is null)
        {
            throw new InvalidOperationException("Invalid interaction result : playable character position missing");
        }
        Character? posCharacter = BoardQuery.GetCell(CurrentGame.Board, characterPosition).Character;
        if (posCharacter is not Character playableCharacter)
        {
            throw new InvalidOperationException("No playable character found at selected position");
        }
        // TODO - loop using a while until the action is completed or cancelled
        bool cancelAction = false;
        _characterActionBuilder = new CharacterActionBuilder(playableCharacter);
        CharacterAction? characterAction = null;
        do
        {
            // TODO - update and use resolver to get possible play positions
            Dictionary<TargetCategory, IReadOnlyList<Position>> legalPositions = [];
            // TODO - get legal results (= can use NoChoice) using resolver
            List<InteractionResultType> legalResults = [
                InteractionResultType.PositionChosen, 
                InteractionResultType.NoChoice, 
                InteractionResultType.CancelAction];
            // TODO - build a request using the positions
            InteractionRequest playCharacterRequest = new InteractionRequest(InteractionType.PositionExpected, null, legalPositions, legalResults);
            // TODO - get characterAction using resolver.BuildAction
        } while (characterAction is null && !cancelAction);
        // TODO - comment
        if (characterAction is CharacterAction playedAction)
        {
            DoAction(currentPhase, playedAction);
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    private async Task RunRecruitmentCharacterAsync(GamePhase currentPhase, List<CharacterType> characterTypes, int characterIdx)
    {
        if (characterIdx >= characterTypes.Count)
        {
            return;
        }
        // TODO - comment
        List<Position> recruitmentPositions = BoardQuery.GetRecruitmentPositions(CurrentGame.Board, currentPhase.PhasePlayer.Team);
        Dictionary<TargetCategory, IReadOnlyList<Position>> legalPositions = [];
        List<InteractionResultType> legalResults = [InteractionResultType.PositionChosen, InteractionResultType.CancelAction];
        legalPositions.Add(TargetCategory.RecruitmentDestination, recruitmentPositions);
        InteractionRequest recruitmentRequest = new InteractionRequest(InteractionType.PositionExpected, null, legalPositions, legalResults);
        InteractionResult recruitmentResult = await _gameFlowListener.OnInputRequired(recruitmentRequest);
        if (recruitmentResult.ResultType == InteractionResultType.PositionChosen)
        {
            if (recruitmentResult.ChosenPosition is not Position recruitmentPosition)
            {
                throw new InvalidOperationException("Invalid interaction result : recruitment position missing");
            }
            Character recruitedCharacter = CharacterFactory.Create(characterTypes[characterIdx], currentPhase.PhasePlayer.Team);
            // We apply the recruitment on the game and history then continue with the next characterIdx
            DoAction(currentPhase, new RecruitmentAction(recruitedCharacter, recruitmentPosition));
            await RunRecruitmentCharacterAsync(currentPhase, characterTypes, characterIdx + 1);

        } else if (recruitmentResult.ResultType == InteractionResultType.CancelAction)
        {
            // If the action is cancelled with no recruited character, we cancel the recruitment alltogether by exiting the function.
            // This will lead to "RunRecruitmentCharacterAsync" starting over with a new card selection request
            if (characterIdx == 0)
            {
                return;
            }
            // We cancel the last recruitment action then restart the recruitment with the previous characterIdx
            UndoLastAction();
            await RunRecruitmentCharacterAsync(currentPhase, characterTypes, characterIdx - 1);
        } else
        {
            throw new InvalidOperationException($"Invalid interaction result : illegal type \"{recruitmentResult.ResultType}\" for character recruitment");
        }
    }

    /// <summary>
    /// Ends the current phase by closing its segment in <see cref="GameHistory"/>.
    /// If the ending phase is a <see cref="GamePhaseType.TurnEnd"/>, also closes the parent <see cref="Turn"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no current phase segment is found in history, or if a <see cref="GamePhaseType.TurnEnd"/>
    /// phase has no matching turn in progress.
    /// </exception>
    private void EndCurrentPhase()
    {
        IPhase? phase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (phase is not Segment segment)
        {
            throw new InvalidOperationException("Each game phase should correspond to a non-null segment in the history");
            
        }
        // At the end of a "turnEnd" phase, we also end the turn
        if (segment.Transition == TransitionTarget.TurnEndPhase)
        {
            if (GameHistoryQuery.GetCurrentTurn(CurrentHistory) is Turn currentTurn)
            {
                segment.End();
                currentTurn.End();
            } else
            {
                throw new InvalidOperationException("No turn found matching current end turn phase");
            }
        } else
        {
            segment.End();
        }
    }

    /// <summary>
    /// Determines the next phase from the current history state, opens the corresponding segment
    /// in <see cref="GameHistory"/>, and notifies the listener via <see cref="IGameFlowListener.OnPhaseChanged"/>.
    /// If the next phase is a <see cref="GamePhaseType.TurnStart"/>, also opens a new <see cref="Turn"/>.
    /// If the next phase is a <see cref="GamePhaseType.Banishment"/>, opens a new <see cref="BanishmentPhase"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a non-turn-boundary phase transition finds no turn in progress in history.
    /// </exception>
    private async Task StartNextPhaseAsync()
    {
        GamePhase nextPhase = PhaseTransitionQuery.GetNextPhase(CurrentHistory, CurrentGame);
        if (nextPhase.PhaseType == GamePhaseType.TurnStart)
        {
            // At the start of a turn, we also start the "startTurn" phase
            Turn nextTurn = new Turn(nextPhase.PhasePlayer.Team);
            nextTurn.Start();
            nextTurn.TurnStartPhase.Start();
        } else if (nextPhase.PhaseType == GamePhaseType.Banishment)
        {
            BanishmentPhase banishmentPhase = new BanishmentPhase(nextPhase.PhasePlayer.Team);
            banishmentPhase.Start();
        } else if (GameHistoryQuery.GetLastEntry(CurrentHistory) is Turn currentTurn)
        {  
            Segment nextSegment = currentTurn.GetPhaseAsSegment(nextPhase.PhaseType);
            nextSegment.Start();
        } else
        {
            throw new InvalidOperationException("All transition besides \"TurnStart\" and \"Banishment\" should imply a turn in progress");
        }
        // We notify the listener the phase changed and wait for an acknowledgement
        await _gameFlowListener.OnPhaseChanged(nextPhase);
    }

    /// <summary>
    /// Applies the given action to <see cref="CurrentGame"/> via the appropriate handler,
    /// appends it to the current phase in <see cref="GameHistory"/>,
    /// then checks whether the game has ended via <see cref="CheckGameEnded"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no current phase is found in history.
    /// </exception>
    private void DoAction(GamePhase currentGamePhase, IGameAction action)
    {
        IActionHandler actionHandler = GameActionHandlerFactory.Create(CurrentGame, action);
        actionHandler.DoAction();
        IPhase? phase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (phase is not IPhase currentPhase)
        {
            throw new InvalidOperationException("Cannot do an action outside of a game phase");
        }
        currentPhase.Actions.Add(action);
        // TODO - comment
        CheckGameEnded(currentGamePhase);
    }

    /// <summary>
    /// Undo the last action's effect on <see cref="CurrentGame"/> via the appropriate handler
    /// then removes it from the current phase in <see cref="GameHistory"/>.
    /// </summary>
    private void UndoLastAction()
    {
        IPhase? phase = GameHistoryQuery.GetCurrentPhase(CurrentHistory);
        if (phase is not IPhase currentPhase || currentPhase.Actions.Count == 0)
        {
            throw new InvalidOperationException("Cannot undo an action outside of a game phase or within an empty phase");
        }
        IGameAction lastAction = currentPhase.Actions[^1];
        currentPhase.Actions.Remove(lastAction);
        IActionHandler actionHandler = GameActionHandlerFactory.Create(CurrentGame, lastAction);
        actionHandler.UndoAction();
    }
}