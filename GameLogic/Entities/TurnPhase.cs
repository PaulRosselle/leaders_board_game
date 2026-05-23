using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class TurnPhase
{
    public TurnPhaseType PhaseType { get; }
    public Player PhasePlayer { get; }

    public TurnPhase(TurnPhaseType phaseType, Player phasePlayer)
    {
        PhaseType = phaseType;
        PhasePlayer = phasePlayer;
    }
}