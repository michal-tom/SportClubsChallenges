namespace SportClubsChallenges.Jobs
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Strava;
    using SportClubsChallenges.Strava.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAthleteActivitiesJob
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly IMapper mapper;

        public GetAthleteActivitiesJob(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.mapper = mapper;
        }

        public async Task Run()
        {
            var athleths = await this.db.Athletes
                .Include(p => p.AthleteStravaToken)
                .Include(p => p.ChallengeParticipants)
                .Include(p => p.ChallengeParticipants.Select(p => p.Challenge))
                .Where(p => p.ChallengeParticipants.Any())
                .ToListAsync();

            foreach(var athlete in athleths)
            {
                var firstAthleteChallenge = athlete.ChallengeParticipants
                    .Select(p => p.Challenge)
                    .Where(p => p.EndDate >= DateTime.Now.Date.AddDays(-3))
                    .OrderBy(p => p.StartDate)
                    .FirstOrDefault();

                if (firstAthleteChallenge?.StartDate != null)
                {
                    await this.GetAthleteActivities(athlete, firstAthleteChallenge.StartDate, DateTimeOffset.Now);
                }
            }
        }

        private async Task GetAthleteActivities(Athlete athlete, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var stravaToken = this.mapper.Map<StravaToken>(athlete.AthleteStravaToken);

            List<Activity> activitesFromStrava = await GetActivitiesFromStrava(stravaToken, startTime);
            List<Activity> activitiesInDb = await GetActivitiesFromDatabase(athlete, startTime);
            
            this.UpdateActivitiesInDatabase(activitesFromStrava, activitiesInDb);

            // TODO: move to service???
            //if (stravaToken.IsRefreshed)
            //{
            //    this.UpdateStravaToken(athlete, stravaToken);
            //}

            await db.SaveChangesAsync();
        }

        private async Task<List<Activity>> GetActivitiesFromStrava(StravaToken stravaToken, DateTimeOffset startTime)
        {
            var stravaActivites = await this.stravaWrapper.GetAthleteActivites(stravaToken, startTime, endTime: null);
            return this.mapper.Map<List<Activity>>(stravaActivites);
        }

        private async Task<List<Activity>> GetActivitiesFromDatabase(Athlete athlete, DateTimeOffset startTime)
        {
            return await this.db.Activities
                .Where(p => p.AthleteId == athlete.Id && p.StartDate >= startTime)
                .ToListAsync();
        }

        private void UpdateActivitiesInDatabase(List<Activity> activitesFromStrava, List<Activity> activitiesInDb)
        {
            foreach (var activityFromStrava in activitesFromStrava)
            {
                var currentActivityInDb = activitiesInDb.FirstOrDefault(p => p.Id == activityFromStrava.Id);
                if (currentActivityInDb == null)
                {
                    this.db.Activities.Add(activityFromStrava);
                    return;
                }

                if (IsActivityChanged(activityFromStrava, currentActivityInDb))
                {
                    UpdateActivity(currentActivityInDb);
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

        private static void UpdateActivity(Activity currentActivityInDb)
        {
            currentActivityInDb.Name = currentActivityInDb.Name;
            currentActivityInDb.ActivityTypeId = currentActivityInDb.ActivityTypeId;
            currentActivityInDb.Duration = currentActivityInDb.Duration;
            currentActivityInDb.Distance = currentActivityInDb.Distance;
            currentActivityInDb.Elevation = currentActivityInDb.Elevation;
            currentActivityInDb.Pace = currentActivityInDb.Pace;
            currentActivityInDb.StartDate = currentActivityInDb.StartDate;
            currentActivityInDb.EndDate = currentActivityInDb.EndDate;
            currentActivityInDb.IsDeleted = false;
        }

        private static void CheckRemovedActivities(List<Activity> activitesFromStrava, List<Activity> activitiesInDb)
        {
            foreach (var currentActivityInDb in activitiesInDb.Where(p => !p.IsDeleted && !activitesFromStrava.Exists(a => a.Id == p.Id)))
            {
                // TODO: test if works
                currentActivityInDb.IsDeleted = true;
            }
        }
    }
}
