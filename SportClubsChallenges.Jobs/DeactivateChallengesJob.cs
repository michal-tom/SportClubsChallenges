namespace SportClubsChallenges.Jobs
{
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class DeactivateChallengesJob
    {
        private readonly SportClubsChallengesDbContext db;

        public DeactivateChallengesJob(SportClubsChallengesDbContext db)
        {
            this.db = db;
        }

        public async Task Run()
        {
            var challengesToDeactivate = await this.db.Challenges
                .Where(p => p.IsActive && p.EndDate.Date < DateTime.Now.Date.AddDays(-1))
                .ToListAsync();

            foreach (var challenge in challengesToDeactivate)
            {
                challenge.IsActive = false;
            }

            await this.db.SaveChangesAsync();
        }
    }
}