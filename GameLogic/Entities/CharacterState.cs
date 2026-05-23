using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class CharacterState
{
    public PlayPermission Playability { get; set; }
    public HashSet<CharacterType> AbilityBlockedBy { get; set; }
    public CharacterType? TransformedInto { get; set; }

    public CharacterState(PlayPermission playability, HashSet<CharacterType> abilityBlockedBy, CharacterType? transformedInto)
    {
        Playability = playability;
        AbilityBlockedBy = abilityBlockedBy;
        TransformedInto = transformedInto;
    }

    public CharacterState(CharacterState refState) : this(refState.Playability, [.. refState.AbilityBlockedBy], refState.TransformedInto)
    {
    }

    public CharacterState()
    {
        // Characters aren't allowed to play by default
        Playability = PlayPermission.Forbidden;
        // Abilities can be blocked by external factors, there are no default blocker
        AbilityBlockedBy = [];
        // Characters can be tranformed by other character, there are no default transformation
        TransformedInto = null;
    }

    /// <summary>
    /// Returns true if AbilityBlockedBy is not empty
    /// </summary>
    public bool IsAbilityBlocked()
    {
        return AbilityBlockedBy.Count > 0;
    }
}