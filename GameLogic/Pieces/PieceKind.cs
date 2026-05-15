namespace LeadersBoardGame.Pieces;

using System;
using LeadersBoardGame.Cards;

public enum PieceKind
{
    Acrobat,
    Archer,
    Assassin,
    Brewmaster,
    Bruiser,
    ClawLauncher,
    Cub,
    Hermit,
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

public static class PieceKindMethods
{
    public static CardKind GetCardKind(this PieceKind pieceKind) => pieceKind switch
    {
        PieceKind.Acrobat => CardKind.Acrobat,
        PieceKind.Archer => CardKind.Archer,
        PieceKind.Assassin => CardKind.Assassin,
        PieceKind.Brewmaster => CardKind.Brewmaster,
        PieceKind.Bruiser => CardKind.Bruiser,
        PieceKind.ClawLauncher => CardKind.ClawLauncher,
        PieceKind.Hermit or PieceKind.Cub => CardKind.HermitAndCub,
        PieceKind.Illusionist => CardKind.Illusionist,
        PieceKind.Jailer => CardKind.Jailer,
        PieceKind.LeaderKing => CardKind.LeaderKing,
        PieceKind.LeaderQueen => CardKind.LeaderQueen,
        PieceKind.Manipulator => CardKind.Manipulator,
        PieceKind.Nemesis => CardKind.Nemesis,
        PieceKind.Protector => CardKind.Protector,
        PieceKind.Rider => CardKind.Rider,
        PieceKind.RoyalGuard => CardKind.RoyalGuard,
        PieceKind.Vizier => CardKind.Vizier,
        PieceKind.Wanderer => CardKind.Wanderer,
        _ => throw new InvalidOperationException($"No card kind found for piece kind {pieceKind}"),
    };
}