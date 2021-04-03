namespace SportClubsChallenges.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;
    using SportClubsChallenges.Utils.Enums;
    using SportClubsChallenges.Utils.Helpers;
    using System.Linq;
    using SportClubsChallenges.AzureQueues;

    public class ChallengeService : IChallengeService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        private readonly IAzureStorageRepository storageRepository;

        public ChallengeService(IMapper mapper, SportClubsChallengesDbContext db, IAzureStorageRepository storageRepository)
        {
            this.mapper = mapper;
            this.db = db;
            this.storageRepository = storageRepository;
        }

        public async Task<List<ChallengeOverviewDto>> GetAllChallenges()
        {
            return await mapper.ProjectTo<ChallengeOverviewDto>(db.Challenges).ToListAsync();
        }

        public async Task<List<ChallengeOverviewDto>> GetAvailableChallenges(long athleteId)
        {
            var challenges = db.Challenges
                .Where(p => p.Club.ClubMembers.Any(p => p.AthleteId == athleteId))
                .OrderByDescending(p => p.StartDate);

            var challengesParticipations = challenges
                .SelectMany(p => p.ChallengeParticipants)
                .Where(p => p.AthleteId == athleteId);

            var challengesOverview = await mapper.ProjectTo<ChallengeOverviewDto>(challenges).ToListAsync();

            challengesOverview.ForEach(
                p => p.IsAthleteRegistred = challengesParticipations.Any(c => c.ChallengeId == p.Id)
            );

            return challengesOverview;
        }

        public async Task<List<ChallengeParticipationDto>> GetChallengeParticipations(long athleteId)
        {
            var challengeParticipations = db.ChallengeParticipants
                .Where(p => p.AthleteId == athleteId)
                .OrderByDescending(p => p.RegistrationDate);

            return await mapper.ProjectTo<ChallengeParticipationDto>(challengeParticipations).ToListAsync();
        }

        public async Task<List<ChallengeRankPositionDto>> GetChallengeRank(long challengeId, long athleteId)
        {
            var challengeParticipations = db.ChallengeParticipants
                .Where(p => p.ChallengeId == challengeId)
                .OrderBy(p => p.Rank);

            var rankList = await mapper.ProjectTo<ChallengeRankPositionDto>(challengeParticipations).ToListAsync();

            rankList.Where(p => p.AthleteId == athleteId).ToList().ForEach(p => p.IsCurrentUserRank = true);

            return rankList;
        }

        public async Task<ChallengeDetailsDto> GetChallenge(long challengeId)
        {
            var entity = await db.Challenges.FirstOrDefaultAsync(p => p.Id == challengeId);
            return mapper.Map<ChallengeDetailsDto>(entity);
        }

        public async Task AddOrEditChallenge(ChallengeDetailsDto dto)
        {
            if (dto.Id != default)
            {
                this.EditChallenge(dto);
            }
            else
            {
                this.AddChallenge(dto);
            }

            await db.SaveChangesAsync();

            if (dto.Id != default)
            {
                await this.UpdateChallengeRank(dto.Id);
            }
        }

        public async Task DeleteChallenge(long challengeId)
        {
            var challengeParticipations = db.ChallengeParticipants.Where(p => p.ChallengeId == challengeId);
            foreach(var parcitipation in challengeParticipations)
            {
                db.ChallengeParticipants.Remove(parcitipation);
            }

            var challengeActivityTypes = db.ChallengeActivityTypes.Where(p => p.ChallengeId == challengeId);
            foreach (var activityType in challengeActivityTypes)
            {
                db.ChallengeActivityTypes.Remove(activityType);
            }

            var challenge = db.Challenges.Find(challengeId);
            db.Challenges.Remove(challenge);

            await db.SaveChangesAsync();
        }

        public async Task LeaveChallenge(long athleteId, long challengeId)
        {
            var challengeParticipation = await db.ChallengeParticipants
                .FirstOrDefaultAsync(p => p.AthleteId == athleteId && p.ChallengeId == challengeId);

            if (challengeParticipation == null)
            {
                return;
            }

            db.ChallengeParticipants.Remove(challengeParticipation);
            await db.SaveChangesAsync();

            await this.UpdateChallengeRank(challengeId);
        }

        public async Task JoinChallenge(long athleteId, long challengeId)
        {
            var challengeParticipants = db.ChallengeParticipants.Where(p => p.ChallengeId == challengeId);

            if (challengeParticipants.Any(p => p.AthleteId == athleteId))
            {
                return;
            }

            var athleteMembershipInChallengeClub = await db.Challenges
                .Where(p => p.Id == challengeId)
                .SelectMany(p => p.Club.ClubMembers)
                .FirstOrDefaultAsync(p => p.AthleteId == athleteId);

            if (athleteMembershipInChallengeClub == null)
            {
                return;
            }

            var challengeParticipation = new ChallengeParticipant { AthleteId = athleteId, ChallengeId = challengeId };
            challengeParticipation.Score = 0;
            challengeParticipation.RegistrationDate = DateTimeOffset.Now;
            challengeParticipation.LastUpdateDate = DateTimeOffset.Now;
            challengeParticipation.Rank = await challengeParticipants.CountAsync(p => p.ChallengeId == challengeId) + 1;

            db.ChallengeParticipants.Add(challengeParticipation);
            await db.SaveChangesAsync();

            await this.UpdateChallengeRank(challengeId);
        }

        public Dictionary<byte, string> GetAvailableChallengeCompetitionTypes()
        {
            return EnumsHelper.GetEnumWithDescriptions<ChallengeCompetitionTypeEnum>();
        }

        public async Task<Dictionary<byte, string>> GetAvailableActivityTypes()
        {
            return await db.ActivityTypes.AsNoTracking().ToDictionaryAsync(p => p.Id, p => p.Name);
        }

        private void AddChallenge(ChallengeDetailsDto dto)
        {
            var entity = mapper.Map<Challenge>(dto);
            entity.CreationDate = DateTimeOffset.Now;
            entity.EditionDate = DateTimeOffset.Now;
            entity.Club = db.Clubs.Find(dto.ClubId);
            entity.Author = db.Athletes.Find(dto.AuthorId);
            entity.ChallengeActivityTypes = new List<ChallengeActivityType>();
            foreach (var activityTypeId in dto.ActivityTypesIds)
            {
                entity.ChallengeActivityTypes.Add(new ChallengeActivityType { ChallengeId = entity.Id, ActivityTypeId = activityTypeId });
            }
            db.Challenges.Add(entity);
        }

        private void EditChallenge(ChallengeDetailsDto dto)
        {
            var entity = db.Challenges.Find(dto.Id);
            mapper.Map(dto, entity);
            entity.EditionDate = DateTimeOffset.Now;
            entity.Club = db.Clubs.Find(dto.ClubId);
            entity.Author = db.Athletes.Find(dto.AuthorId);
            if (entity.ChallengeActivityTypes.Any())
            {
                entity.ChallengeActivityTypes.Clear();
            }
            foreach (var activityTypeId in dto.ActivityTypesIds)
            {
                entity.ChallengeActivityTypes.Add(new ChallengeActivityType { ChallengeId = entity.Id, ActivityTypeId = activityTypeId });
            }
        }

        private async Task UpdateChallengeRank(long challengeId)
        {
            var queuesClient = new AzureQueuesClient(this.storageRepository);
            await queuesClient.UpdateChallengeRank(challengeId);
        }
    }
}