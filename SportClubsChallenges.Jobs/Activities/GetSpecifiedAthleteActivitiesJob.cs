namespace SportClubsChallenges.Jobs.Activities
{
    using AutoMapper;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Strava;
    using System.Threading.Tasks;

    public class GetSpecifiedAthleteActivitiesJob : GetAthleteActivitiesBaseJob
    {
        private readonly SportClubsChallengesDbContext db;

        public GetSpecifiedAthleteActivitiesJob(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper) :
            base(db, stravaWrapper, tokenService, mapper)
        {
            this.db = db;
        }

        public async Task Run(long athleteId)
        {
            var athlete = this.db.Athletes.Find(athleteId);
            if (athlete == null)
            {
                return;
            }

            await GetAthleteActivities(athlete);
        }
    }
}