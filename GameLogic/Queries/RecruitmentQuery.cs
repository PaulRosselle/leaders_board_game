namespace LeadersBoardGame.GameLogic.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.HistoryEntries;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;

// TODO - add to documentation
public static class RecruitmentQuery
{
    public static bool CanRecruit(GameHistory history, Game game, TeamColor team)
    {
        return GetRecruitmentLimit(history, game, team) > 0 && BoardQuery.GetRecruitmentPositions(game.Board, team).Count > 0;
    }

    /// <summary>
    /// Returns the number of cards that can be recruited during the current recruitment phase
    /// </summary>
    private static int GetRecruitmentLimit(GameHistory history, Game game, TeamColor team)
    {
        int recruitmentCount = GetRecruitedCards(game, team, addLeaders: false).Count;
        // The 2nd player is allowed to recruit twice during their first recruitment phase
        if (recruitmentCount == 0 && team != history.Config.FirstPlayer.Team)
        {
            return 2;
        }
        // A full team is made of of 5 character cards (a leader card + 4 recruited cards)
        const int fullTeamSize = 4;
        // 1 card per phase can be recruited until the team is full
        return recruitmentCount < fullTeamSize ? 1 : 0;
    }

    /// <summary>
    /// Returns the list of cards recruitable during the current recruitment phase
    /// </summary>
    public static List<CharacterCard> GetRecruitableCards(GameHistory history, Game game)
    {
        // First we get the team associated with the current entry since it will define the recruitment cells
        TeamColor? currentEntryTeam = GameHistoryQuery.GetLastEntryTeam(history);
        if (currentEntryTeam is not TeamColor currentRecruitmentTeam)
        {
            throw new InvalidOperationException("Cannot recruit without a valid history entry");
        }
        // No recruitment can be made without at least one available recruitment cell
        List<Position> recruitmentCells = BoardQuery.GetRecruitmentPositions(game.Board, currentRecruitmentTeam);
        if (recruitmentCells.Count == 0)
        {
            return [];
        }

        // We only return a list if a recruitment phase is in progress
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(history);
        if (currentPhase is RecruitmentPhase recruitmentPhase)
        {
            // We get every card already recruited this phase
            HashSet<CharacterCard> recruitedCharacterCard = [];
            foreach (RecruitmentAction recruitmentAction in recruitmentPhase.Actions.Cast<RecruitmentAction>())
            {
                recruitedCharacterCard.Add(recruitmentAction.Character.CharacterType.GetCharacterCard());
            }
            // If the recruited card cound is still under the limit, we can return every card with 
            // characters able to be placed on the board
            if (recruitedCharacterCard.Count < GetRecruitmentLimit(history, game, currentRecruitmentTeam))
            {
                // TODO - handle game mode 
                List<CharacterCard> recruitableCards = [];
                foreach (CharacterCard recruitableCard in game.RecruitableCards)
                {
                    if (CharacterTypeExtension.GetCharacterTypesMatchingCard(recruitableCard).Count <= recruitmentCells.Count)
                    {
                        recruitableCards.Add(recruitableCard);
                    }                    
                }
                return recruitableCards;
            }
        }
        // By default we return an empty list
        return [];
    }

    /// <summary>
    /// Returns cards recruited within a given <paramref name="team"/>
    /// </summary>
    public static HashSet<CharacterCard> GetRecruitedCards(Game game, TeamColor team, bool addLeaders)
    {
        HashSet<CharacterCard> teamRecruitmentCards = [];
        foreach (Character character in game.RecruitedCharacters)
        {
            if (character.Team == team && (addLeaders || !character.CharacterType.GetCharacterCard().IsLeader()))
            {
                teamRecruitmentCards.Add(character.CharacterType.GetCharacterCard());
            }
        }
        return teamRecruitmentCards;
    }
}