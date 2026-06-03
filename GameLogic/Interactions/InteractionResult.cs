namespace LeadersBoardGame.GameLogic.Interactions;

using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Entities;

/// <summary>
/// Response returned by the external caller to in reply to an <see cref="InteractionRequest"/>.
/// Carries the response type and, depending on that type, an optional chosen value.
/// </summary>
public record InteractionResult(
    InteractionResultType ResultType,
    CharacterCard? ChosenCard,
    Position? ChosenPosition
);
