namespace SportClubsChallenges.Domain.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;

    public class ActivityService : IActivityService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        public ActivityService(IMapper mapper, SportClubsChallengesDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<List<ActivityDto>> GetAthleteActivities(long athleteId, int? maxCount = null)
        {
            var activities = db.Activities
                .Where(p => p.AthleteId == athleteId && !p.IsDeleted)
                .OrderByDescending(p => p.StartDate)
                .Take(maxCount == null ? int.MaxValue : maxCount.Value);

            return await mapper.ProjectTo<ActivityDto>(activities).ToListAsync();
        }

        public async Task<List<ActivityDto>> GetAllActivities()
        {
            return await mapper.ProjectTo<ActivityDto>(db.Activities.OrderByDescending(p => p.StartDate)).ToListAsync();
        }
    }
}