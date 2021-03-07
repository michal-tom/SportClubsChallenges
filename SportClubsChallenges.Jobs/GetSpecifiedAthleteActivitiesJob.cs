namespace SportClubsChallenges.Jobs
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

        public async Task Run(long atheletId)
        {
            // get all athletes from active challenges
            var athlete = this.db.Athletes.Find(atheletId);
            if (athlete == null)
            {
                return;
            }

            await this.GetAthleteActivities(athlete);
        }
    }
}