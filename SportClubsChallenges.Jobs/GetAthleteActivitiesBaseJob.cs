namespace SportClubsChallenges.Jobs
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Strava;
    using SportClubsChallenges.Strava.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAthleteActivitiesBaseJob
    {
        // TODO: get from config
        private readonly DateTimeOffset applicationStartDate = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public GetAthleteActivitiesBaseJob(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        protected async Task GetAthleteActivities(Athlete athlete)
        {
            var stravaToken = this.tokenService.GetStravaToken(athlete);

            var activitesFromStrava = await this.GetActivitiesFromStrava(stravaToken, this.applicationStartDate);
            if (activitesFromStrava == null || !activitesFromStrava.Any())
            {
                return;
            }

            var activitiesInDb = await this.GetActivitiesFromDatabase(athlete.Id, this.applicationStartDate);

            this.UpdateActivitiesInDatabase(activitesFromStrava, activitiesInDb);

            await db.SaveChangesAsync();
        }

        private async Task<List<Activity>> GetActivitiesFromStrava(StravaToken stravaToken, DateTimeOffset startTime)
        {
            try
            {
                var stravaActivites = await this.stravaWrapper.GetAthleteActivites(stravaToken, startTime, endTime: null);
                return this.mapper.Map<List<Activity>>(stravaActivites);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<List<Activity>> GetActivitiesFromDatabase(long athleteId, DateTimeOffset startTime)
        {
            return await this.db.Activities.Where(p => p.AthleteId == athleteId && p.StartDate >= startTime).ToListAsync();
        }

        private void UpdateActivitiesInDatabase(IList<Activity> activitesFromStrava, IList<Activity> activitiesInDb)
        {
            foreach (var activityFromStrava in activitesFromStrava)
            {
                var currentActivityInDb = activitiesInDb.FirstOrDefault(p => p.Id == activityFromStrava.Id);
                if (currentActivityInDb == null)
                {
                    this.db.Activities.Add(activityFromStrava);
                    continue;
                }

                if (IsActivityChanged(activityFromStrava, currentActivityInDb))
                {
                    UpdateActivity(activityFromStrava, currentActivityInDb);
                }
            }

            CheckRemovedActivities(activitesFromStrava, activitiesInDb);
        }

        private static bool IsActivityChanged(Activity activityFromStrava, Activity currentActivityInDb)
        {
            return currentActivityInDb.Name != activityFromStrava.Name
                || currentActivityInDb.ActivityTypeId != activityFromStrava.ActivityTypeId
                || currentActivityInDb.Duration != activityFromStrava.Duration
                || currentActivityInDb.IsDeleted;
        }

        private static void UpdateActivity(Activity activityFromStrava, Activity currentActivityInDb)
        {
            currentActivityInDb.Name = activityFromStrava.Name;
            currentActivityInDb.ActivityTypeId = activityFromStrava.ActivityTypeId;
            currentActivityInDb.Duration = activityFromStrava.Duration;
            currentActivityInDb.Distance = activityFromStrava.Distance;
            currentActivityInDb.Elevation = activityFromStrava.Elevation;
            currentActivityInDb.Pace = activityFromStrava.Pace;
            currentActivityInDb.StartDate = activityFromStrava.StartDate;
            currentActivityInDb.EndDate = activityFromStrava.EndDate;
            currentActivityInDb.IsDeleted = false;
        }

        private static void CheckRemovedActivities(IList<Activity> activitesFromStrava, IList<Activity> activitiesInDb)
        {
            foreach (var currentActivityInDb in activitiesInDb.Where(p => !p.IsDeleted && !activitesFromStrava.Any(a => a.Id == p.Id)))
            {
                // TODO: test if works
                currentActivityInDb.IsDeleted = true;
            }
        }
    }
}
