namespace SportClubsChallenges.Domain.Services
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.AzureQueues;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AthleteService : IAthleteService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly ITokenService tokenService;

        private readonly IIdentityService identityService;

        private readonly IAzureStorageRepository storageRepository;

        public AthleteService(
            SportClubsChallengesDbContext db,
            ITokenService tokenService,
            IIdentityService identityService,
            IAzureStorageRepository storageRepository)
        {
            this.db = db;
            this.tokenService = tokenService;
            this.identityService = identityService;
            this.storageRepository = storageRepository;
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

            this.tokenService.UpdateStravaToken(athlete, properties);

            this.identityService.UpdateIdentity(identity, athlete);

            await this.QueueUpdateAthleteClubs(athlete.Id);
        }

        private async Task<Athlete> UpdateAthleteData(ClaimsIdentity identity, long athleteId)
        {
            var athlete = this.db.Athletes.Include(p => p.AthleteStravaToken).FirstOrDefault(p => p.Id == athleteId);
            if (athlete == null)
            {
                athlete = new Athlete { 
                    Id = athleteId, 
                    CreationDate = DateTimeOffset.Now, 
                    AthleteStravaToken = new AthleteStravaToken()
                };
                this.db.Athletes.Add(athlete);
            }

            this.identityService.UpdateAthleteDataFromIdentity(identity, athlete);

            athlete.LastLoginDate = DateTimeOffset.Now;

            await db.SaveChangesAsync();

            return athlete;
        }

        private async Task QueueUpdateAthleteClubs(long athleteId)
        {
            var queuesClient = new AzureQueuesClient(this.storageRepository);
            await queuesClient.SyncAthleteClubs(athleteId);
        }
    }
}