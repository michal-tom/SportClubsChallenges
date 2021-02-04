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
            this.UpdateAthleteIdentity(identity, athlete);
            this.UpdateStravaToken(athlete, tokens);

            await db.SaveChangesAsync();

            //var stravaToken = this.mapper.Map<StravaToken>(athlete.AthleteStravaToken);

            //await this.UpdateAthleteActivities(athleteId.Value, stravaToken, new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero), endTime: null);
            //await this.UpdateAthleteClubs(athleteId.Value, stravaToken);

            //if (stravaToken.IsRefreshed)
            //{
            //    this.UpdateStravaToken(athlete, stravaToken);
            //}

            //await db.SaveChangesAsync();
        }

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
        }

        private async Task UpdateAthleteActivities(long athleteId, StravaToken stravaToken, DateTimeOffset startTime, DateTimeOffset? endTime)
        {
            var stravaSummaryActivites = await this.stravaWrapper.GetAthleteActivites(stravaToken, startTime, endTime: null);
            var activitesInStrava = this.mapper.Map<List<Activity>>(stravaSummaryActivites);

            var activitiesInDb = await this.db.Activities.Where(p => p.AthleteId == athleteId && p.StartDate >= startTime).ToListAsync();

            foreach (var activityInStrava in activitesInStrava)
            {
                var currentActivityInDb = activitiesInDb.FirstOrDefault(p => p.Id == activityInStrava.Id);
                if (currentActivityInDb == null)
                {
                    this.db.Activities.Add(activityInStrava);
                }
                else if (currentActivityInDb.Name != activityInStrava.Name 
                    || currentActivityInDb.ActivityTypeId != activityInStrava.ActivityTypeId
                    || currentActivityInDb.Duration != activityInStrava.Duration
                    || currentActivityInDb.IsDeleted)
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
            }

            foreach (var currentActivityInDb in activitiesInDb.Where(p => !p.IsDeleted && !activitesInStrava.Exists(a => a.Id == p.Id)))
            {
                // TODO: test if works
                if (!endTime.HasValue || endTime.Value > currentActivityInDb.StartDate)
                {
                    currentActivityInDb.IsDeleted = true;
                }
            }
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

        private void UpdateAthleteIdentity(ClaimsIdentity identity, Athlete athlete)
        {
            if (!identity.HasClaim(p => p.Type == ClaimTypes.Name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, $"{athlete.FirstName} {athlete.LastName}"));
            }

            if (athlete.IsAdmin)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }
        }

        private void UpdateStravaToken(Athlete athlete, IEnumerable<AuthenticationToken> authenticationTokens)
        {
            var stravaToken = new StravaToken
            {
                AccessToken = authenticationTokens.FirstOrDefault(p => p.Name == "access_token")?.Value,
                RefreshToken = authenticationTokens.FirstOrDefault(p => p.Name == "refresh_token")?.Value,
                TokenType = authenticationTokens.FirstOrDefault(p => p.Name == "token_type")?.Value
            };

            var expiresAt = authenticationTokens.FirstOrDefault(p => p.Name == "expires_at")?.Value;
            stravaToken.ExpirationDate = DateTimeOffset.TryParse(expiresAt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var expiration)
                ? expiration
                : DateTimeOffset.UtcNow;

            this.UpdateStravaToken(athlete, stravaToken);
        }

        private void UpdateStravaToken(Athlete athlete, StravaToken token)
        {
            if (athlete.AthleteStravaToken == null)
            {
                athlete.AthleteStravaToken = new AthleteStravaToken();
            }

            athlete.AthleteStravaToken.LastUpdateDate = DateTimeOffset.UtcNow;
            athlete.AthleteStravaToken.AccessToken = token.AccessToken;
            athlete.AthleteStravaToken.RefreshToken = token.RefreshToken;
            athlete.AthleteStravaToken.TokenType = token.TokenType;
            athlete.AthleteStravaToken.ExpirationDate = token.ExpirationDate;
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