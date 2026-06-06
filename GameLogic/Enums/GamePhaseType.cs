namespace LeadersBoardGame.GameLogic.Enums;

using System;

public enum GamePhaseType
{
    Banishment,
    TurnStart,
    Actions,
    Recruitment,
    TurnEnd
}

public static class GamePhaseTypeExtension
{
    public static readonly GamePhaseType[] AllPhaseTypes = Enum.GetValues<GamePhaseType>();

    public static TransitionTarget GetTransition(this GamePhaseType gamePhaseType) => gamePhaseType switch
    {
        GamePhaseType.Banishment => TransitionTarget.BanishmentPhase,
        GamePhaseType.TurnStart => TransitionTarget.TurnStartPhase,
        GamePhaseType.Actions => TransitionTarget.BanishmentPhase,
        GamePhaseType.Recruitment => TransitionTarget.RecruitmentPhase,
        GamePhaseType.TurnEnd => TransitionTarget.TurnEndPhase,
        _ => throw new InvalidOperationException($"No transition found matching phase type {gamePhaseType}"),
    };

    public static GamePhaseType GetFromTransitionTarget(TransitionTarget transitionTarget)
    {
        foreach (GamePhaseType phaseType in AllPhaseTypes)
        {
            if (phaseType.GetTransition() == transitionTarget)
            {
                return phaseType;
            }
        }
        throw new InvalidOperationException($"No phase type found matching transition {transitionTarget}");
    }
}