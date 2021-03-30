namespace SportClubsChallenges.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.AzureQueues;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;
    using SportClubsChallenges.Utils.Helpers;

    public class AthleteService : IAthleteService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly ITokenService tokenService;

        private readonly IIdentityService identityService;

        private readonly IAzureStorageRepository storageRepository;

        private readonly IMapper mapper;

        public AthleteService(
            SportClubsChallengesDbContext db,
            ITokenService tokenService,
            IIdentityService identityService,
            IAzureStorageRepository storageRepository,
            IMapper mapper)
        {
            this.db = db;
            this.tokenService = tokenService;
            this.identityService = identityService;
            this.storageRepository = storageRepository;
            this.mapper = mapper;
        }

        public async Task OnAthleteLogin(ClaimsIdentity identity, AuthenticationProperties properties)
        {
            var athleteId = this.identityService.GetAthleteIdFromIdentity(identity);
            if (athleteId == default)
            {
                // TODO: log error
                return;
            }

            var athlete = await this.UpdateAthleteData(identity, athleteId);

            this.tokenService.UpdateStravaToken(athleteId, properties);

            this.identityService.UpdateIdentity(identity, athlete);

            if (athlete.LastLoginDate != athlete.FirstLoginDate)
            {
                return;
            }

            // get clubs and activities from Strava during first login to app
            await this.QueueUpdateInitialAthleteData(athleteId);
        }

        public async Task<List<AthleteDto>> GetAllAthletes()
        {
            return await mapper.ProjectTo<AthleteDto>(this.db.Athletes).ToListAsync();
        }

        public async Task<AthleteDto> GetAthlete(long id)
        {
            var athlete = await db.Athletes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return mapper.Map<AthleteDto>(athlete);
        }

        public async Task EditAthlete(AthleteDto dto)
        {
            var entity = db.Athletes.Find(dto.Id);
            entity.FirstLoginDate = dto.FirstLoginDate;
            entity.LastLoginDate = dto.LastLoginDate;
            entity.LastSyncDate = dto.LastSyncDate;
            entity.IsAdmin = dto.IsAdmin;
            await db.SaveChangesAsync();
        }

        public OverallStatsDto GetAthleteActivitiesTotalStats(long id)
        {
            var activities = db.Activities.AsNoTracking().Where(p => p.AthleteId == id && p.IsDeleted == false).AsEnumerable();

            return new OverallStatsDto()
            {
                TotalStats = this.GetPeriodStats(activities, DateTimeOffset.MinValue),
                YearStats = this.GetPeriodStats(activities, new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero)),
                MonthStats = this.GetPeriodStats(activities, new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, TimeSpan.Zero)),
                WeekStats = this.GetPeriodStats(activities, new DateTimeOffset(TimeHelper.GetStartOfCurrentWeek())),
                FirstActivityDateTime = activities.OrderBy(p => p.StartDate.Ticks).FirstOrDefault()?.StartDate,
                PreferedActivityTypeId = activities.GroupBy(p => p.ActivityTypeId).OrderByDescending(group => group.Count()).FirstOrDefault()?.Key ?? null
            };
        }

        public PeriodStatsDto GetAthleteActivitiesLastSevenDaysStats(long id)
        {
            var dayWeekAgo = new DateTimeOffset(DateTime.Today.AddDays(-7));
            var activities = db.Activities.AsNoTracking().Where(p => p.AthleteId == id && p.IsDeleted == false && p.StartDate > dayWeekAgo).AsEnumerable();

            return this.GetPeriodStats(activities, dayWeekAgo);
        }

        private async Task<Athlete> UpdateAthleteData(ClaimsIdentity identity, long athleteId)
        {
            var currentDateTime = DateTimeOffset.Now;

            var athlete = this.db.Athletes.Include(p => p.AthleteStravaToken).FirstOrDefault(p => p.Id == athleteId);
            if (athlete == null)
            {
                athlete = new Athlete { 
                    Id = athleteId, 
                    FirstLoginDate = currentDateTime, 
                    AthleteStravaToken = new AthleteStravaToken()
                };
                this.db.Athletes.Add(athlete);
            }

            this.identityService.UpdateAthleteDataFromIdentity(identity, athlete);

            athlete.LastLoginDate = currentDateTime;

            await db.SaveChangesAsync();

            return athlete;
        }

        private async Task QueueUpdateInitialAthleteData(long athleteId)
        {
            var queuesClient = new AzureQueuesClient(this.storageRepository);
            await queuesClient.SyncAthleteClubs(athleteId);
            await queuesClient.SyncAthleteActivities(athleteId);
        }

        private PeriodStatsDto GetPeriodStats(IEnumerable<Activity> activities, DateTimeOffset startDate)
        {
            var periodActivities = activities.Where(p => p.StartDate >= startDate);

            return new PeriodStatsDto()
            {
                Count = periodActivities.Count(),
                Distance = periodActivities.Sum(p => p.Distance),
                Duration = periodActivities.Sum(p => p.Duration),
                Elevation = periodActivities.Sum(p => p.Elevation)
            };
        }
    }
}