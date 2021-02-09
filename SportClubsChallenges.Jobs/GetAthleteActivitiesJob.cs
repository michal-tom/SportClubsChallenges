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

    public class GetAthleteActivitiesJob
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public GetAthleteActivitiesJob(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        public async Task Run()
        {
            var athlethsInActiveChallenges = this.db.Athletes
                .Include(p => p.AthleteStravaToken)
                .Where(p => p.ChallengeParticipants.Any(c => c.Challenge.IsActive))
                .ToList();

            foreach(var athlete in athlethsInActiveChallenges)
            {
                var firstAthleteChallenge = athlete.ChallengeParticipants
                    .Select(p => p.Challenge)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.StartDate)
                    .FirstOrDefault();

                if (firstAthleteChallenge?.StartDate != null)
                {
                    await this.GetAthleteActivitiesAsync(athlete, firstAthleteChallenge.StartDate);
                }
            }
        }

        private async Task GetAthleteActivitiesAsync(Athlete athlete, DateTimeOffset startTime)
        {
            var stravaToken = this.tokenService.GetStravaToken(athlete);

            var activitesFromStrava = await GetActivitiesFromStrava(stravaToken, startTime);
            if (activitesFromStrava == null || !activitesFromStrava.Any())
            {
                return;
            }

            var activitiesInDb = GetActivitiesFromDatabase(athlete.Id, startTime);
            
            this.UpdateActivitiesInDatabase(activitesFromStrava, activitiesInDb);

            db.SaveChanges();
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

        private IQueryable<Activity> GetActivitiesFromDatabase(long athleteId, DateTimeOffset startTime)
        {
            return this.db.Activities.Where(p => p.AthleteId == athleteId && p.StartDate >= startTime);
        }

        private void UpdateActivitiesInDatabase(List<Activity> activitesFromStrava, IQueryable<Activity> activitiesInDb)
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

        private static void CheckRemovedActivities(List<Activity> activitesFromStrava, IQueryable<Activity> activitiesInDb)
        {
            foreach (var currentActivityInDb in activitiesInDb.Where(p => !p.IsDeleted && !activitesFromStrava.Exists(a => a.Id == p.Id)))
            {
                // TODO: test if works
                currentActivityInDb.IsDeleted = true;
            }
        }
    }
}
