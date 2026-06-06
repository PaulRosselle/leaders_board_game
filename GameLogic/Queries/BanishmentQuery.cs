namespace LeadersBoardGame.GameLogic.Queries;

using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Entities;
using System.Collections.Generic;

public static class BanishmentQuery
{
    public static bool CanBanish(Game game)
    {
        int banishmentCount = game.BanishedCards.Count;
        int blackRecruitmentCardsCount = RecruitmentQuery.GetRecruitedCards(game, TeamColor.Black, false).Count;
        int whiteRecruitmentCardsCount = RecruitmentQuery.GetRecruitedCards(game, TeamColor.White, false).Count;
        return (blackRecruitmentCardsCount == whiteRecruitmentCardsCount) &&
            (banishmentCount == 0 || banishmentCount == 2) && 
            (blackRecruitmentCardsCount < banishmentCount + 2);
    }

    public static List<CharacterCard> GetBanishableCards(Game game)
    {
        return game.RecruitableCards;
    }
}