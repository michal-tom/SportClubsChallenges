namespace SportClubsChallenges.Domain.Services
{
    using System.Collections.Generic;
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

        public async Task<ClubDto> GetClub(long id)
        {
            var entity = await db.Clubs.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return mapper.Map<ClubDto>(entity);
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