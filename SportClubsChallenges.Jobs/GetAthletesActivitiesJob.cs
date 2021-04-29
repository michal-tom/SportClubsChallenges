namespace SportClubsChallenges.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Strava;
    using SportClubsChallenges.Strava;

    public class GetAthletesActivitiesJob
    {
        private readonly DateTimeOffset applicationStartDate = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private readonly SportClubsChallengesDbContext db;

        private readonly IActivityService activityService;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public GetAthletesActivitiesJob(SportClubsChallengesDbContext db, IActivityService activityService, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.activityService = activityService;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        public async Task Run(long? athleteId = null)
        {
            if (athleteId.HasValue)
            {
                await this.GetSpecifiedAthleteActivities(athleteId.Value);
                return;
            }

            await this.GetAllAthletesActivities();
        }

        private async Task GetSpecifiedAthleteActivities(long athleteId)
        {
            var athlete = this.db.Athletes.Find(athleteId);
            if (athlete == null)
            {
                return;
            }

            await GetAthleteActivities(athlete);
        }

        private async Task GetAllAthletesActivities()
        {
            // get all athletes from active challenges
            var athlethsInActiveChallenges = await this.db.Athletes
                .Include(p => p.AthleteStravaToken)
                .Where(p => p.IsAdmin || p.ChallengeParticipants.Any(c => c.Challenge.IsActive))
                .ToListAsync();

            foreach (var athlete in athlethsInActiveChallenges)
            {
                await this.GetAthleteActivities(athlete);
            }
        }

        private async Task GetAthleteActivities(Athlete athlete)
        {
            var stravaToken = this.tokenService.GetStravaToken(athlete);

            var activitesFromStrava = await this.GetActivitiesFromStrava(stravaToken, applicationStartDate);
            if (activitesFromStrava == null || !activitesFromStrava.Any())
            {
                athlete.LastSyncDate = DateTimeOffset.Now;
                await this.db.SaveChangesAsync();
                return;
            }

            await this.activityService.SyncActivities(athlete.Id, activitesFromStrava, applicationStartDate);

            athlete.LastSyncDate = DateTimeOffset.Now;
            await this.db.SaveChangesAsync();
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
    }
}