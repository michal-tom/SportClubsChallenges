namespace SportClubsChallenges.Domain.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Strava;
    using SportClubsChallenges.Strava.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AthleteService : IAthleteService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IIdentityService identityService;

        private readonly IMapper mapper;

        public AthleteService(
            SportClubsChallengesDbContext db,
            IStravaApiWrapper stravaWrapper,
            ITokenService tokenService,
            IIdentityService identityService,
            IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.identityService = identityService;
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

            this.tokenService.UpdateStravaToken(athlete, properties);

            this.identityService.UpdateIdentity(identity, athlete);

            var stravaToken = this.mapper.Map<StravaToken>(athlete.AthleteStravaToken);

            await this.UpdateAthleteClubs(athleteId, stravaToken);
        }

        // TODO: move to club service or add event
        private async Task UpdateAthleteClubs(long athleteId, StravaToken stravaToken)
        {
            var stravaSummaryClubs = await this.stravaWrapper.GetAthleteClubs(stravaToken);
            var clubsInStrava = this.mapper.Map<List<Club>>(stravaSummaryClubs);

            var clubsInDb = await this.db.Clubs.ToListAsync();
            var athleteClubMemebershipInDb = await this.db.ClubMembers.Where(p => p.AthleteId == athleteId).ToListAsync();

            foreach (var clubInStrava in clubsInStrava)
            {
                var currentClubInDb = clubsInDb.FirstOrDefault(p => p.Id == clubInStrava.Id);
                if (currentClubInDb == null)
                {
                    this.db.Clubs.Add(clubInStrava);
                }
                else if (currentClubInDb.Name != clubInStrava.Name
                    || currentClubInDb.MembersCount != clubInStrava.MembersCount
                    || currentClubInDb.IconUrl != clubInStrava.IconUrl)
                {
                    currentClubInDb.Name = clubInStrava.Name;
                    currentClubInDb.MembersCount = clubInStrava.MembersCount;
                    currentClubInDb.IconUrl = clubInStrava.IconUrl;
                }

                var currentAthleteClubMemebershipInDb = athleteClubMemebershipInDb.FirstOrDefault(p => p.ClubId == clubInStrava.Id);
                if (currentAthleteClubMemebershipInDb == null)
                {
                    this.db.ClubMembers.Add(new ClubMember { AthleteId = athleteId, ClubId = clubInStrava.Id });
                }
            }

            await db.SaveChangesAsync();
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
    }
}