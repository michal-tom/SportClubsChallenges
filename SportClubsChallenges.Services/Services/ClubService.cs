namespace SportClubsChallenges.Domain.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;

    public class ClubService : IClubService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        public ClubService(IMapper mapper, SportClubsChallengesDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<List<ClubDto>> GetAllClubs()
        {
            return await mapper.ProjectTo<ClubDto>(db.Clubs.AsNoTracking()).ToListAsync();
        }

        public async Task<List<ClubDto>> GetAthleteClubs(long athleteId)
        {
            var clubs = this.db.ClubMembers.Where(p => p.AthleteId == athleteId).Select(p => p.Club);
            return await mapper.ProjectTo<ClubDto>(clubs).ToListAsync();
        }

        public async Task<ClubDto> GetClub(long id)
        {
            var entity = await db.Clubs.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return mapper.Map<ClubDto>(entity);
        }

        public async Task<List<AthleteDto>> GetMembers(long id)
        {
            var clubMembers = this.db.ClubMembers.Where(p => p.ClubId == id).Select(p => p.Athlete);
            return await mapper.ProjectTo<AthleteDto>(clubMembers).ToListAsync();
        }

        public async Task<List<ChallengeOverviewDto>> GetChallenges(long id)
        {
            var clubChallenges = this.db.Challenges.Where(p => p.ClubId == id);
            return await mapper.ProjectTo<ChallengeOverviewDto>(clubChallenges).ToListAsync();
        }

        public async Task AddClub(ClubDto dto)
        {
            var entity = mapper.Map<Club>(dto);
            db.Clubs.Add(entity);
            await db.SaveChangesAsync();
        }
 
        public async Task UpdatClub(ClubDto dto)
        {
            var entity = db.Clubs.Find(dto.Id);
            mapper.Map(dto, entity);
            await db.SaveChangesAsync();
        }
    }
}