namespace SportClubsChallenges.Jobs.Activities
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Strava;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetActiveAthletesActivitiesJob : GetAthleteActivitiesBaseJob
    {
        private readonly SportClubsChallengesDbContext db;

        public GetActiveAthletesActivitiesJob(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper) :
            base(db, stravaWrapper, tokenService, mapper)
        {
            this.db = db;
        }

        public async Task Run()
        {
            // get all athletes from active challenges
            var athlethsInActiveChallenges = await this.db.Athletes
                .Include(p => p.AthleteStravaToken)
                .Where(p => p.ChallengeParticipants.Any(c => c.Challenge.IsActive))
                .ToListAsync();

            foreach (var athlete in athlethsInActiveChallenges)
            {
                await this.GetAthleteActivities(athlete);
            }
        }
    }
}
