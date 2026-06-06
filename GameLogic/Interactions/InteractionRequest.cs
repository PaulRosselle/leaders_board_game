namespace LeadersBoardGame.GameLogic.Interactions;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Entities;

/// <summary>
/// Context token emitted whenever an interactive step is required.
/// Describes the expected input type, the corresponding legal values, and the exhaustive list
/// of accepted response types.
/// </summary>
public record InteractionRequest(
    InteractionType Type,
    IReadOnlyList<CharacterCard>? LegalCards,
    IReadOnlyDictionary<TargetCategory, IReadOnlyList<Position>>? LegalPositions,
    IReadOnlyList<InteractionResultType> LegalResults
);
