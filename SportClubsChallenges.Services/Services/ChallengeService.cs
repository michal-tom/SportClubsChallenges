namespace SportClubsChallenges.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Utils.Helpers;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;
    using SportClubsChallenges.Model.Enums;
    using System.Linq;

    public class ChallengeService : IChallengeService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        public ChallengeService(IMapper mapper, SportClubsChallengesDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<List<ChallengeOverviewDto>> GetAllChallenges()
        {
            var challenges = db.Challenges.AsNoTracking();
            return await mapper.ProjectTo<ChallengeOverviewDto>(challenges).ToListAsync();
        }

        public async Task<List<ChallengeParticipationDto>> GetAthleteChallengeParticipations(long athleteId)
        {
            var challengeParticipations = db.ChallengeParticipants.AsNoTracking()
                .Where(p => p.AthleteId == athleteId)
                .OrderByDescending(p => p.RegistrationDate);
            return await mapper.ProjectTo<ChallengeParticipationDto>(challengeParticipations).ToListAsync();
        }

        public async Task<ChallengeDetailsDto> GetChallenge(long id)
        {
            var entity = await db.Challenges.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return mapper.Map<ChallengeDetailsDto>(entity);
        }

        public async Task AddChallenge(ChallengeDetailsDto dto)
        {
            var entity = mapper.Map<Challenge>(dto);
            entity.CreationDate = DateTime.Now;
            // TODO: pass creator id
            entity.OwnerId = 1;
            db.Challenges.Add(entity);
            await db.SaveChangesAsync();
        }

        public async Task UpdatChallenge(ChallengeDetailsDto dto)
        {
            var entity = db.Challenges.Find(dto.Id);
            mapper.Map(dto, entity);
            await db.SaveChangesAsync();
        }

        public async Task DeleteChallenge(long id)
        {
            var challenge = db.Challenges.Find(id);
            db.Challenges.Remove(challenge);
            await db.SaveChangesAsync();
        }

        public async Task<Dictionary<long, string>> GetAvailableClubs()
        {
            return await db.Clubs.AsNoTracking().ToDictionaryAsync(p => p.Id, p => p.Name);
        }

        public Dictionary<byte, string> GetAvailableChallengeTypes()
        {
            return EnumsHelper.GetEnumWithDescriptions<ChallengeTypeEnum>();
        }

        public async Task<Dictionary<byte, string>> GetAvailableActivityTypes()
        {
            return await db.ActivityTypes.AsNoTracking().ToDictionaryAsync(p => p.Id, p => p.Name);
        }
    }
}