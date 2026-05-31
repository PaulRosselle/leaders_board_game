namespace LeadersBoardGame.GameLogic.Entities;

using LeadersBoardGame.GameLogic.Enums;

public class GamePhase
{
    public GamePhaseType PhaseType { get; }
    public TeamColor Team { get; }

    public GamePhase(GamePhaseType phaseType, TeamColor team)
    {
        Team = team;
        PhaseType = phaseType;
    }
}