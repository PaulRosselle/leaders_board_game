namespace LeadersBoardGame.GameLogic.Interactions;

using System.Threading.Tasks;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;


/// <summary>
/// Observer interface to be implemented by an external orchestrating a game session.
/// All methods are asynchronous, allowing the caller to drive timing for animations and UI transitions.
/// </summary>
public interface IGameFlowListener
{
    /// <summary>
    /// Fired once when the game session starts, before the first turn.
    /// </summary>
    Task OnGameStarted(Game game);

    /// <summary>
    /// Fired once when the game session ends. Carries the winning team.
    /// </summary>
    Task OnGameEnded(Player winner);

    /// <summary>
    /// Fired whenever the game enters a new phase. Carries the incoming phase as context.
    /// </summary>
    Task OnPhaseChanged(GamePhase phase);

    /// <summary>
    /// Fired after each automatic action executed during a non-interactive phase,
    /// allowing the caller to animate the corresponding board change.
    /// </summary>
    Task OnAutomaticActionExecuted(IGameAction action);

    /// <summary>
    /// Fired whenever the game requires an input from the caller.
    /// The <see cref="InteractionRequest"/> carries the expected input type, legal values,
    /// and accepted response types. The caller must return an <see cref="InteractionResult"/>
    /// that satisfies the request contract.
    /// </summary>
    Task<InteractionResult> OnInputRequired(InteractionRequest request);
}
