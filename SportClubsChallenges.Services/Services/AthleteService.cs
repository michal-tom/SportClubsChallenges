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
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AthleteService : IAthleteService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly IMapper mapper;

        public AthleteService(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.mapper = mapper;
        }

        public async Task OnAthleteLogin(ClaimsIdentity identity, IEnumerable<AuthenticationToken> tokens)
        {
            var athleteId = GetAthleteIdFromIdentity(identity);
            if (!athleteId.HasValue)
            {
                // TODO: log error
                return;
            }

            var athlete = this.UpdateAthleteData(identity, athleteId.Value);
            this.UpdateStravaToken(athlete, tokens);

            await db.SaveChangesAsync();

            // TODO: get activites + get clubs
            var stravaToken = this.mapper.Map<StravaToken>(athlete.AthleteStravaToken);
            await this.stravaWrapper.RetrieveAccessTokenAsync(stravaToken);

            var stravaSummaryActivites = this.stravaWrapper.GetAthleteActivites(stravaToken, startTime: null, endTime: null);
            this.mapper.Map<List<Activity>>(stravaSummaryActivites);
        }

        private Athlete UpdateAthleteData(ClaimsIdentity identity, long athleteId)
        {
            var athlete = this.db.Athletes.Include(p => p.AthleteStravaToken).FirstOrDefault(p => p.Id == athleteId);
            if (athlete == null)
            {
                athlete = new Athlete { Id = athleteId, CreationDate = DateTimeOffset.Now };
                this.db.Athletes.Add(athlete);
            }

            athlete.LastLoginDate = DateTimeOffset.Now;
            athlete.FirstName = GetClaimValueFromIdentity(identity, ClaimTypes.GivenName);
            athlete.LastName = GetClaimValueFromIdentity(identity, ClaimTypes.Surname);
            athlete.Gender = GetClaimValueFromIdentity(identity, ClaimTypes.Gender);
            athlete.Country = GetClaimValueFromIdentity(identity, ClaimTypes.Country);
            athlete.City = GetClaimValueFromIdentity(identity, StravaConsts.CityClaimType);
            athlete.IconUrlLarge = GetClaimValueFromIdentity(identity, StravaConsts.LargeIconClaimType);
            athlete.IconUrlMedium = GetClaimValueFromIdentity(identity, StravaConsts.MediumIconClaimType);

            return athlete;
        }

        private void UpdateStravaToken(Athlete athlete, IEnumerable<AuthenticationToken> tokens)
        {
            if (athlete.AthleteStravaToken == null)
            {
                athlete.AthleteStravaToken = new AthleteStravaToken();
            }

            athlete.AthleteStravaToken.LastUpdateDate = DateTimeOffset.UtcNow;
            athlete.AthleteStravaToken.AccessToken = tokens.FirstOrDefault(p => p.Name == "access_token").Value;
            athlete.AthleteStravaToken.RefreshToken = tokens.FirstOrDefault(p => p.Name == "refresh_token").Value;
            athlete.AthleteStravaToken.TokenType = tokens.FirstOrDefault(p => p.Name == "token_type").Value;

            var expiresAt = tokens.FirstOrDefault(p => p.Name == "expires_at").Value;
            if (DateTimeOffset.TryParse(expiresAt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var expiration))
            {
                athlete.AthleteStravaToken.ExpirationDate = expiration;
            }
            else
            {
                athlete.AthleteStravaToken.ExpirationDate = DateTimeOffset.UtcNow;
            }
        }

        private static long? GetAthleteIdFromIdentity(ClaimsIdentity identity)
        {
            var identifier = GetClaimValueFromIdentity(identity, ClaimTypes.NameIdentifier);
            if(!long.TryParse(identifier, out long athleteId))
            {
                return null;
            }

            return athleteId;
        }

        private static string GetClaimValueFromIdentity(ClaimsIdentity identity, string type)
        {
            var firstName = identity.Claims.FirstOrDefault(c => c.Type == type);
            return firstName?.Value ?? string.Empty;
        }
    }
}