namespace SportClubsChallenges.Domain.Services
{
    using System;
    using System.Globalization;
    using System.Linq;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Strava;

    public class TokenService : ITokenService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        public TokenService(SportClubsChallengesDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public StravaToken GetStravaToken(Athlete athlete)
        {
            var token = this.mapper.Map<StravaToken>(athlete.AthleteStravaToken);
            token.OnTokenRefresh += OnStravaTokenRefresh;
            return token;
        }

        public void UpdateStravaToken(long athleteId, AuthenticationProperties properties)
        {
            var authenticationTokens = properties.GetTokens();

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

            this.UpdateStravaToken(athleteId, stravaToken);
        }

        public void OnStravaTokenRefresh(object sender, object args)
        {
            if (!(sender is StravaToken token) || string.IsNullOrEmpty(token.AccessToken) || string.IsNullOrEmpty(token.RefreshToken))
            {
                return;
            }

            if (token == null || string.IsNullOrEmpty(token.AccessToken) || string.IsNullOrEmpty(token.RefreshToken))
            {
                return;
            }

            var athleteStravaToken = this.db.AthleteStravaTokens.FirstOrDefault(p => p.Id == token.DatabaseId);
            if (athleteStravaToken == null)
            {
                return;
            }

            athleteStravaToken.LastUpdateDate = DateTimeOffset.UtcNow;
            athleteStravaToken.AccessToken = token.AccessToken;
            athleteStravaToken.RefreshToken = token.RefreshToken;
            athleteStravaToken.TokenType = token.TokenType;
            athleteStravaToken.ExpirationDate = token.ExpirationDate;

            this.db.SaveChanges();
        }

        private void UpdateStravaToken(long athleteId, StravaToken token)
        {
            var athlete = this.db.Athletes.Include(p => p.AthleteStravaToken).FirstOrDefault(p => p.Id == athleteId);
            if (athlete.AthleteStravaToken == null)
            {
                athlete.AthleteStravaToken = new AthleteStravaToken();
            }

            athlete.AthleteStravaToken.LastUpdateDate = DateTimeOffset.UtcNow;
            athlete.AthleteStravaToken.AccessToken = token.AccessToken;
            athlete.AthleteStravaToken.RefreshToken = token.RefreshToken;
            athlete.AthleteStravaToken.TokenType = token.TokenType;
            athlete.AthleteStravaToken.ExpirationDate = token.ExpirationDate;

            this.db.SaveChanges();
        }
    }
}