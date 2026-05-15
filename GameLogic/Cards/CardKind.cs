namespace LeadersBoardGame.Cards;

using System;

public enum CardKind
{
    Acrobat,
    Archer,
    Assassin,
    Brewmaster,
    Bruiser,
    ClawLauncher,
    HermitAndCub,
    Illusionist,
    Jailer,
    LeaderKing,
    LeaderQueen,
    Manipulator,
    Nemesis,
    Protector,
    Rider,
    RoyalGuard,
    Vizier,
    Wanderer
}

public static class CardKindMethods
{
    public static AbilityKind[] GetAbilityKinds(this CardKind cardKind) => cardKind switch
    {
        CardKind.Acrobat => new AbilityKind[] { AbilityKind.Active },
        CardKind.Archer => new AbilityKind[] { AbilityKind.Passive },
        CardKind.Assassin => new AbilityKind[] { AbilityKind.Passive },
        CardKind.Brewmaster => new AbilityKind[] { AbilityKind.Active },
        CardKind.Bruiser => new AbilityKind[] { AbilityKind.Active },
        CardKind.ClawLauncher => new AbilityKind[] { AbilityKind.Active },
        CardKind.HermitAndCub => new AbilityKind[] { AbilityKind.Special },
        CardKind.Illusionist => new AbilityKind[] { AbilityKind.Active },
        CardKind.Jailer => new AbilityKind[] { AbilityKind.Passive },
        CardKind.LeaderKing or CardKind.LeaderQueen => [],
        CardKind.Manipulator => new AbilityKind[] { AbilityKind.Active },
        CardKind.Nemesis => new AbilityKind[] { AbilityKind.Special },
        CardKind.Protector => new AbilityKind[] { AbilityKind.Passive },
        CardKind.Rider => new AbilityKind[] { AbilityKind.Active },
        CardKind.RoyalGuard => new AbilityKind[] { AbilityKind.Active },
        CardKind.Vizier => new AbilityKind[] { AbilityKind.Passive },
        CardKind.Wanderer => new AbilityKind[] { AbilityKind.Active },
        _ => throw new InvalidOperationException($"No ability kinds found for card kind {cardKind}"),
    };

    /// <summary>
    /// Get the leaderKind associated with the card. Returns null if there is none
    /// </summary>
    public static LeaderKind? GetLeaderKind(this CardKind cardKind) => cardKind switch
    {
        CardKind.LeaderKing => LeaderKind.King,
        CardKind.LeaderQueen => LeaderKind.Queen,
        _ => null,
    };

    public static bool IsLeader(this CardKind cardKind)
    {
        return GetLeaderKind(cardKind) != null;
    }
    
    public static bool CanBeRecruited(this CardKind cardKind)
    {
        // Leaders start the game directly on the board and cant be recruited afterward
        return !IsLeader(cardKind);
    }
}