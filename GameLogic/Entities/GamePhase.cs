namespace LeadersBoardGame.GameLogic.Entities;

using LeadersBoardGame.GameLogic.Enums;

public class GamePhase
{
    public GamePhaseType PhaseType { get; }
    public Player PhasePlayer { get; }

    public GamePhase(GamePhaseType phaseType, Player phasePlayer)
    {
        PhasePlayer = phasePlayer;
        PhaseType = phaseType;
    }
}