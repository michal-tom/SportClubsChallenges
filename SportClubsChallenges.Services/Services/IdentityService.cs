namespace SportClubsChallenges.Domain.Services
{
    using System.Linq;
    using System.Security.Claims;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Strava;

    public class IdentityService : IIdentityService
    {
        public long GetAthleteIdFromIdentity(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return default(long);
            }
            
            var identifier = GetClaimValueFromIdentity(identity, ClaimTypes.NameIdentifier);
            if (!long.TryParse(identifier, out long athleteId))
            {
                return default(long);
            }

            return athleteId;
        }

        public void UpdateIdentity(ClaimsIdentity identity, Athlete athlete)
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

        public void UpdateAthleteDataFromIdentity(ClaimsIdentity identity, Athlete athlete)
        {
            athlete.FirstName = GetClaimValueFromIdentity(identity, ClaimTypes.GivenName);
            athlete.LastName = GetClaimValueFromIdentity(identity, ClaimTypes.Surname);
            athlete.Gender = GetClaimValueFromIdentity(identity, ClaimTypes.Gender);
            athlete.Country = GetClaimValueFromIdentity(identity, ClaimTypes.Country);
            athlete.City = GetClaimValueFromIdentity(identity, StravaConsts.CityClaimType);
            athlete.IconUrlLarge = GetClaimValueFromIdentity(identity, StravaConsts.LargeIconClaimType);
            athlete.IconUrlMedium = GetClaimValueFromIdentity(identity, StravaConsts.MediumIconClaimType);
        }

        private static string GetClaimValueFromIdentity(ClaimsIdentity identity, string type)
        {
            var firstName = identity.Claims.FirstOrDefault(c => c.Type == type);
            return firstName?.Value ?? string.Empty;
        }
    }
}
